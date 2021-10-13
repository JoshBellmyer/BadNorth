using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Settings
{
    [NonSerialized] string path;
    int playerId;
    public float zoomSensitivity = 1;
    public float rotateSensitivity = 1;
    public float cursorSensitivity = 1;

    public Settings(int playerId)
    {
        this.playerId = playerId;
        path = GetPath(playerId);
        Save();
    }

    public static Settings Load(int playerId)
    {
        string path = GetPath(playerId);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return JsonUtility.FromJson<Settings>(json);
            }
        }
        return new Settings(playerId);
    }

    private static string GetPath(int playerId)
    {
        return Application.persistentDataPath + "/Player" + playerId + "_settings.json";
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(json);
        }
    }
}
