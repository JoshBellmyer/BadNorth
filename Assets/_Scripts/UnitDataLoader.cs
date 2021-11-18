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
        UnitData[] datas = Resources.LoadAll<UnitData>(UNIT_DATA_PATH);
        foreach(UnitData data in datas)
        {
            dataMap[data.type] = data;
        }
    }
}
