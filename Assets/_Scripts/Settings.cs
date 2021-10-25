using System;
using System.IO;
using UnityEngine;

[Serializable]
public class Settings
{
    public int playerId;
    public float zoomSensitivity = 0.01f;
    public float rotateSensitivity = 50.0f;
    public float cursorSensitivity = 1;

    public Settings(int playerId)
    {
        this.playerId = playerId;
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
        using (StreamWriter writer = new StreamWriter(GetPath(playerId)))
        {
            writer.Write(json);
        }
    }
}
