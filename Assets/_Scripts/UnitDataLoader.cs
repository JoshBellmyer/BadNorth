using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UnitDataLoader
{
    static readonly string UNIT_DATA_PATH = "UnitData/";
    static Dictionary<UnitType, UnitData> dataMap;

    public static UnitData GetUnitData(UnitType type)
    {
        if(dataMap == null)
        {
            LoadDataMap();
        }

        try
        {
            return dataMap[type];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public static void LoadDataMap()
    {
        dataMap = new Dictionary<UnitType, UnitData>();
        IEnumerable<string> files = Directory.GetFiles("Assets/Resources/" + UNIT_DATA_PATH)
            .Where(f => !f.Contains(".meta"))
            .Select(f => Path.GetFileNameWithoutExtension(f));
        foreach(string file in files)
        {
            UnitData data = Resources.Load<UnitData>(UNIT_DATA_PATH + file);
            dataMap[data.type] = data;
        }
    }
}
