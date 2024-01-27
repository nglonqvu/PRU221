using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FileUtility
{
    public static T LoadJsonFromFile<T>(string filePath)
    {
        string jsonText = File.ReadAllText(filePath);
        var data = JsonUtility.FromJson<T>(jsonText);
        return data;
    }

    public static T LoadJsonFromFile<T>(TextAsset jsonFile)
    {
        var data = JsonUtility.FromJson<T>(jsonFile.text);
        return data;
    }
}
