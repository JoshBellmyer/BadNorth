using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public static void Load(ISavable savable, string path)
    {
        string filePath = Path.Combine(Application.persistentDataPath, path);
        if (File.Exists(filePath))
        {
            Type type = savable.GetSaveDataType();
            var data = Activator.CreateInstance(type);
            (data as SaveData).LoadFromJson(File.ReadAllText(filePath));

            foreach (var field in type.GetFields())
            {
                var targetField = savable.GetType().GetField(field.Name);
                targetField.SetValue(savable, field.GetValue(data));
            }
        }

        savable.OnFinishLoad();
    }

    public static void Save(ISavable savable, string path)
    {
        Type type = savable.GetSaveDataType();
        var data = Activator.CreateInstance(type);
        foreach (var field in type.GetFields())
        {
            var targetField = savable.GetType().GetField(field.Name);
            field.SetValue(data, targetField.GetValue(savable));
        }
        File.WriteAllText(Path.Combine(Application.persistentDataPath, path), (data as SaveData).ToJson());
    }
}

public interface ISavable
{
    /// <summary>
    /// Gets the type of the SaveData class that describes the data that will be saved to file. 
    /// The fields in the data class should have the same name as the fields in the implementing class
    /// </summary>
    /// <returns></returns>
    Type GetSaveDataType();
    /// <summary>
    /// Called when the ISavable is done loading
    /// </summary>
    void OnFinishLoad();
}
