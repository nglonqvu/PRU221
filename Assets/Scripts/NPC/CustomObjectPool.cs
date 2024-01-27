using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class CustomObjectPool
{
    private IObjectPool<GameObject> _objectPool;

    private GameObject _targetPrefab;

    private bool collectionCheck = true;

    // Maximum object that can be render at one point of time
    // 160 for out-of-screen objects
    // 160 for in-screen objects
    // 160 for objects that are about to be rendered
    private int maxPoolSize = 480;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if(_objectPool == null)
            {
                _objectPool = new ObjectPool<GameObject>(
                    CreatePoolItem,
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyGameObject,
                    collectionCheck, maxPoolSize); ;
            }

            return _objectPool;
        }
    }

    public CustomObjectPool(GameObject prefab)
    {
        _targetPrefab = prefab;
    }

    private GameObject CreatePoolItem()
    {
        var newObject = GameObject.Instantiate(_targetPrefab);
        return newObject;
    }

    private void OnReturnedToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void OnTakeFromPool(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    private void OnDestroyGameObject(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }
}

