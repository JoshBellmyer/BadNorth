using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class GameSettingsScreen : UIScreen
{
    [SerializeField] Dropdown tileSetSelection;

    Dictionary<int, TileSet> valueTypeMap;

    private void Start()
    {
        valueTypeMap = new Dictionary<int, TileSet>();
        tileSetSelection.options.Clear();
        int valueCount = 0;
        valueTypeMap.Add(valueCount, null);
        tileSetSelection.options.Add(new OptionData("Random"));
        valueCount++;
        foreach (TileSet tileSet in Resources.LoadAll<TileSet>("TileSets"))
        {
            if(tileSet != null)
            {
                valueTypeMap.Add(valueCount, tileSet);
                tileSetSelection.options.Add(new OptionData(tileSet.name));
                valueCount++;
            }
        }
    }

    public void OnBack()
    {
        Game.instance.terrainSettings.tileSet = valueTypeMap[tileSetSelection.value];
        manager.SetUIScreen("Title Screen");
    }
}
