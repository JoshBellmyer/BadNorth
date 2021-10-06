using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    [SerializeField] List<GameObject> prefabList;
    Dictionary<Type, GameObject> prefabMap = new Dictionary<Type, GameObject>();
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
    }
    public GameObject GetPrefabOfType(Type type)
    {
        if (!prefabMap.ContainsKey(type))
        {
            foreach (var prefab in prefabList)
            {
                if (prefab.GetComponent(type) != null)
                {
                    prefabMap.Add(type, prefab);
                    return prefab;
                }
            }
            return null;
        } else
        {
            return prefabMap[type];
        }
        
    }
}
