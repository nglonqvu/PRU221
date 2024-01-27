using System;

[Serializable]
public class MapCohesion
{
	// Current map id
	public int Id;

	// Store id of maps that can be connected to current map
	public int[] SuitableMaps;
}

