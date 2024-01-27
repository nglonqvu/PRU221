using System;

[Serializable]
public class MapPattern
{
	// To identify current pattern and to determine their next pattern
	public int Id;

	// Map pattern length, not to be confused with data length
	public int MapLen;

	// Data convertible to a list objects and their position
	public int[] Data;
}

