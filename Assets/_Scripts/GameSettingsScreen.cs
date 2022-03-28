using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class GameSettingsScreen : UIScreen
{
    [SerializeField] string savePath;
    [SerializeField] Dropdown tileSetSelection;

    string fullPath;

    private void Start()
    {
        fullPath = Path.Combine(Application.persistentDataPath, savePath);

        tileSetSelection.options.Clear();
        int valueCount = 0;
        tileSetSelection.options.Add(new OptionData("Random"));
        valueCount++;
        foreach (TileSet tileSet in Resources.LoadAll<TileSet>("TileSets"))
        {
            if(tileSet != null)
            {
                tileSetSelection.options.Add(new OptionData(tileSet.name));
                valueCount++;
            }
        }
    }

    public void OnBack()
    {
        TerrainGenerator.TerrainGeneratorData terrainGeneratorData;
        if (File.Exists(fullPath))
        {
            terrainGeneratorData = JsonUtility.FromJson<TerrainGenerator.TerrainGeneratorData>(File.ReadAllText(fullPath));
        }
        else
        {
            terrainGeneratorData = new TerrainGenerator.TerrainGeneratorData();
            terrainGeneratorData.randomizeSeed = true;
        }
        terrainGeneratorData.tileSetName = tileSetSelection.options[tileSetSelection.value].text;
        File.WriteAllText(fullPath, JsonUtility.ToJson(terrainGeneratorData));
        manager.SetUIScreen("Title Screen");
    }
}
