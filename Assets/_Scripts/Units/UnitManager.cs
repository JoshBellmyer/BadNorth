using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    private static Dictionary<UnitType, GameObject> prefabMap = new Dictionary<UnitType, GameObject>();
    private static List<NavMeshAgent> dummyActive = new List<NavMeshAgent>();
    private static List<NavMeshAgent> dummyInactive = new List<NavMeshAgent>();

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
        InitializeDummies(20);
    }

    public GameObject GetPrefabOfType (UnitType type) {
        if (!prefabMap.ContainsKey(type)) {
            Debug.LogError(string.Format("Prefab of type {0} not found!", type));
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

    public static void DeactivateDummy (NavMeshAgent agent) {
        if (!dummyActive.Contains(agent)) {
            return;
        }

        agent.gameObject.SetActive(false);

        dummyActive.Remove(agent);
        dummyInactive.Add(agent);
    }

    public static NavMeshAgent GetDummyAgent () {
        if (dummyInactive.Count < 1) {
            return null;
        }

        NavMeshAgent agent = dummyInactive[0];
        agent.gameObject.SetActive(true);

        dummyInactive.Remove(agent);
        dummyActive.Add(agent);

        return agent;
    }

    private void InitializeDummies (int amount) {
        GameObject dummyContainer = new GameObject("Dummy Agents");
        NavMeshAgent dummy = Resources.Load<NavMeshAgent>("DummyAgent");

        for (int i = 0; i < amount; i++) {
            NavMeshAgent newAgent = Instantiate<NavMeshAgent>(dummy);
            newAgent.transform.SetParent(dummyContainer.transform);
            newAgent.gameObject.SetActive(false);
            dummyInactive.Add(newAgent);
        }

        dummyActive.Clear();
    }

    public static Type UnitEnumToType (UnitType unitType) {
        string str = $"{unitType.ToString()}Unit";
        Type type;

        try {
            type = Type.GetType(str);
        }
        catch (TypeLoadException e) {
            return null;
        }

        return type;
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













