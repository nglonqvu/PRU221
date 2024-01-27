using System;

public class LoadMapCohesion : ICommand
{
    private readonly Action<MapCohesionFile> _loadedHandler;

    public LoadMapCohesion(Action<MapCohesionFile> callback)
    {
        _loadedHandler = callback;
    }

    public void Execute()
    {
        var mapCohesion = ResourceLoader.LoadMapCohesion();
        _loadedHandler.Invoke(mapCohesion);
    }
}

