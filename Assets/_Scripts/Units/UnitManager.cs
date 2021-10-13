using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    private static Dictionary<UnitType, GameObject> prefabMap = new Dictionary<UnitType, GameObject>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        InitializePrefabs();
    }

    public GameObject GetPrefabOfType (UnitType type) {
        if (!prefabMap.ContainsKey(type)) {
            return null;
        }

        return prefabMap[type];
    }

    public GameObject GetPrefabOfType (Type type) {
        string typeStr = type.ToString();
        typeStr = typeStr.Substring(0, typeStr.Length - 4);
        UnitType unitType = (UnitType)Enum.Parse(typeof(UnitType), typeStr);

        return GetPrefabOfType(unitType);
    }

    private void InitializePrefabs () {
        foreach (UnitType ut in Enum.GetValues(typeof(UnitType))) {
            GameObject unit = Resources.Load<GameObject>($"Units/{ut.ToString()}");

            if (unit != null) {
                prefabMap.Add(ut, unit);
            }
        }
    }
}


public enum UnitType {
    Default = 0,
    Sword = 1,
    Axe = 2,
    Archer = 3,
    Pike = 4,
    Shield = 5,
    Ladder = 6,
}













