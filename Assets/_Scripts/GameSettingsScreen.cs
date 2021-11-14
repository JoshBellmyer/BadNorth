using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class GameSettingsScreen : UIScreen
{
    [SerializeField] Dropdown tileSetSelection;

    Dictionary<int, TileSetType> valueTypeMap;

    private void Start()
    {
        valueTypeMap = new Dictionary<int, TileSetType>();
        tileSetSelection.options.Clear();
        int valueCount = 0;
        foreach (TileSetType type in Enum.GetValues(typeof(TileSetType)))
        {
            if(type == TileSetType.Random)
            {
                valueTypeMap.Add(valueCount, type);
                tileSetSelection.options.Add(new OptionData("Random"));
                valueCount++;
                continue;
            }

            TileSet tileSet = TileSetLoader.GetTileSet(type);
            if(tileSet != null)
            {
                valueTypeMap.Add(valueCount, type);
                tileSetSelection.options.Add(new OptionData(tileSet.tileSetName));
                valueCount++;
            }
        }
    }

    public void OnBack()
    {
        Game.instance.selectedTileSetType = valueTypeMap[tileSetSelection.value];
        Debug.Log("Set TileSet to be " + Game.instance.selectedTileSetType);
        manager.SetUIScreen("Title Screen");
    }
}
