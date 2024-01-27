using System;

public class LoadMapPattern : ICommand
{
    private readonly Action<MapPatternFile> _loadedHandler;

    public LoadMapPattern(Action<MapPatternFile> callback)
    {
        _loadedHandler = callback;
    }

    public void Execute()
    {
        var mapPatterns = ResourceLoader.LoadMapPattern();
        _loadedHandler.Invoke(mapPatterns);
    }
}

