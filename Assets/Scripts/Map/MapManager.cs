using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using System.Threading.Tasks;

[Serializable]
class Placeable
{
    public int BlockCode;
    public BlockType BlockType;
}

// Screen size: 20*10
// 
// Base platform size: 20*2
// 
// => The remaining area that the map can be rendered is:
// (20*10) - (20*2) = (20*8)
public class MapManager : MonoBehaviour
{
    // Renderable screen area
    private const int PLAY_SCREEN_HEIGHT = 8;
    private const int PLAY_SCREEN_WIDTH = 20;

    private const int CHUNK_WIDTH = 10;

    // The screen use its center as its pivot,
    // so it goes from -10 -> 10 on x axis and -5 -> 5 on y axis
    // We need to offset those different so we can place blocks at the right place
    private const int SCREEN_OFFSET_X = -10;
    // Offset y is a little different because it have the platform at the bottom
    // Calculated by screen offset - platform size
    private const int SCREEN_OFFSET_Y = -3;

    // GameObject have the same problem with screen, calculated from their center to their edge
    // We need to offset those different so we can place blocks at the right place
    private const float OBJECT_OFFSET = 0.5f;

    // Chunk size to divide the map pattern into
    private const int CHUNK_SIZE = 5;

    // Reference to NPC Manager to access its ObjectPools
    [SerializeField]
    private NPCManager _npcManager;

    [SerializeField]
    private List<Placeable> _placeables = new();

    private Dictionary<int, BlockType> _placeableCodeMap = new();

    // Data for map patterns and map cohesions
    private List<MapPattern> _mapPatterns;
    private List<MapCohesion> _mapCohesions;

    // Data for map streaming
    private Queue<MapPattern> _mapPatternQueue = new();
    private MapPattern _currentMapPattern;
    private Queue<MapPattern> _currentMapQueue;
    private Queue<MapPattern> _nextMapQueue;

    // Cache chunks for loaded map patterns
    private Dictionary<int, Queue<MapPattern>> _chunkMap;

    private bool renderable = true;
    private float _mapCoverTime;

    private void Awake()
    {
        ICommand loadMapPatternCommand = new LoadMapPattern(SetMapPatterns);
        loadMapPatternCommand.Execute();

        ICommand loadMapCohesionCommand = new LoadMapCohesion(SetMapCohesions);
        loadMapCohesionCommand.Execute();
    }

    private void Start()
    {
        // Initialize things
        _currentMapQueue = new();
        _nextMapQueue = new();
        _chunkMap = new();

        // Map game blocks into a dictionary
        foreach (Placeable placeable in _placeables)
        {
            _placeableCodeMap.Add(placeable.BlockCode, placeable.BlockType);
        }

        _currentMapPattern = _mapPatterns.Find(pattern => pattern.Id == 0);
        _currentMapQueue = ChunkMapPattern(_currentMapPattern);
    }

    private void FixedUpdate()
    {
        // Time it take to current map pattern to finish on screen
        if (renderable)
        {
            renderable = false;
            var pattern = _currentMapQueue.Dequeue();
            _mapCoverTime = pattern.MapLen / (GameConst.PLATFORM_SPEED * GameConst.SPEED_SCALE);

            GenerateMap(pattern);
        }

        if (_currentMapQueue.Count <= 0)
        {
            _currentMapPattern = LoadNextMap();
            _currentMapQueue = ChunkMapPattern(_currentMapPattern);
        }

        _mapCoverTime -= Time.deltaTime;
        if (_mapCoverTime > 0)
        {
            return;
        }

        renderable = true;
    }

    private void ValidateMap(MapPattern mapPattern)
    {
        var data = mapPattern.Data;
        var length = mapPattern.MapLen;
        if (data.Length % length != 0)
        {
            throw new Exception($"Map data is not compatible with length: {length}");
        }

        var row = data.Length / length;
        if (row > PLAY_SCREEN_HEIGHT)
        {
            throw new Exception("Unable to render map with over 8 rows");
        }
    }

    private void GenerateMap(MapPattern mapPattern)
    {
        var data = mapPattern.Data;
        var length = mapPattern.MapLen;

        if (data.Length == 0)
        {
            return;
        }

        var row = data.Length / length;

        List<Action> cleanupActions = new();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var currentCode = data[i * length + j];
                if (currentCode == 0)
                {
                    continue;
                }

                if (_placeableCodeMap.TryGetValue(currentCode, out var blockType))
                {
                    var instantiatePos = new Vector3(
                        transform.position.x + j + SCREEN_OFFSET_X + OBJECT_OFFSET + PLAY_SCREEN_WIDTH,
                        transform.position.y + i + SCREEN_OFFSET_Y + OBJECT_OFFSET,
                        transform.position.z
                    );

                    var currentObjectPool = _npcManager.GetObjectPool(blockType);
                    currentObjectPool.Pool.Get(out var placeable);

                    placeable.transform.position = instantiatePos;

                    cleanupActions.Add(() => currentObjectPool.Pool.Release(placeable));
                }
            }
        }

        // Start cleaning up out of screen objects after the map pattern is finished
        StartCoroutine(
            ReleaseAfterSeconds(
                (PLAY_SCREEN_WIDTH + mapPattern.MapLen) /
                GameConst.PLATFORM_SPEED * GameConst.SPEED_SCALE, cleanupActions
            )
        );
    }

    private MapPattern LoadNextMap()
    {
        // Calculate map pattern from current map pattern
        int nextMapId = 0;

        var currentMapCohesion = _mapCohesions.Find(mc => mc.Id == _currentMapPattern.Id);

        // Random the next map patterns from the suitable map pool
        // If current cohesion is not set, default the next pattern to Id 0 (null pattern)
        if (currentMapCohesion != null)
        {
            nextMapId = currentMapCohesion.SuitableMaps.Length == 1 ?
                currentMapCohesion.SuitableMaps[0] :
                currentMapCohesion.SuitableMaps[
                    UnityEngine.Random.Range(0, currentMapCohesion.SuitableMaps.Length)
                ];
        }

        return _mapPatterns.Find(mp => mp.Id == nextMapId);
    }

    private Queue<MapPattern> ChunkMapPattern(MapPattern mapPattern)
    {
        // Check chunk cache in case the map is loaded before
        if (_chunkMap.TryGetValue(mapPattern.Id, out var mapPatterns))
        {
            return new(mapPatterns);
        }

        // Special check for null map pattern
        if (mapPattern.Data.Length == 0)
        {
            var result = new Queue<MapPattern>();
            result.Enqueue(mapPattern);
            return result;
        }

        // Chunk function
        mapPatterns = new();
        int numberOfRow = mapPattern.Data.Length / mapPattern.MapLen;
        int numberOfChunk = mapPattern.MapLen % CHUNK_SIZE == 0 ?
            mapPattern.MapLen / CHUNK_SIZE :
            mapPattern.MapLen / CHUNK_SIZE + 1;

        var remainingSize = mapPattern.MapLen;
        for (int i = 0; i < numberOfChunk; i++)
        {
            var currentChunkSize = remainingSize >= CHUNK_SIZE ? CHUNK_SIZE : remainingSize;
            remainingSize -= currentChunkSize;

            // Calculate chunk start index based on chunk index
            // var chunkStartIndex = currentChunkSize == CHUNK_SIZE ?
            //     i * CHUNK_SIZE :
            //     (i - 1) * CHUNK_SIZE + currentChunkSize;
            var chunkStartIndex = i * CHUNK_SIZE;

            var data = new List<int>();
            for (int a = 0; a < numberOfRow; a++)
            {
                for (int b = chunkStartIndex; b < chunkStartIndex + currentChunkSize; b++)
                {
                    data.Add(mapPattern.Data[a * mapPattern.MapLen + b]);
                }
            }

            // Add to queue after calculation
            mapPatterns.Enqueue(new MapPattern
            {
                Id = -1,
                MapLen = currentChunkSize,
                Data = data.ToArray()
            });
        }

        // Cache chunk data
        _chunkMap.Add(mapPattern.Id, new(mapPatterns));

        return mapPatterns;
    }

    private IEnumerator ReleaseAfterSeconds(float seconds, List<Action> actions)
    {
        yield return new WaitForSecondsRealtime(seconds);

        foreach (Action action in actions)
        {
            action.Invoke();
        }
    }

    private void SetMapPatterns(MapPatternFile mapPatterns)
    {
        _mapPatterns = new(mapPatterns.MapPatterns);
    }

    private void SetMapCohesions(MapCohesionFile mapCohesions)
    {
        _mapCohesions = new(mapCohesions.MapCohesions);
    }
}

