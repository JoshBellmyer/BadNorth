using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;
    private Dictionary<string, HashSet<Unit>> dictionary = new Dictionary<string, HashSet<Unit>>();


    private void Awake () {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }

    public void Add (string team, Unit unit) {
        if (!dictionary.ContainsKey(team)) {
            dictionary.Add(team, new HashSet<Unit>());
        }

        if (!dictionary[team].Contains(unit)) {
            dictionary[team].Add(unit);
        }
    }

    public void Remove (string team, Unit unit) {
        dictionary[team].Remove(unit);

        if (dictionary[team].Count < 1) {
            Game.instance.OnGameOver(team);
        }
    }

    public HashSet<Unit> GetOnTeam(string team) {
        HashSet<Unit> set = new HashSet<Unit>();

        if (!dictionary.ContainsKey(team)) {
            return set;
        }

        set.UnionWith(dictionary[team]);

        return set;
    }

    public HashSet<Unit> GetNotOnTeam(string team) {
        HashSet<Unit> set = new HashSet<Unit>();

        foreach (var v in dictionary.Keys) {
            if (!v.Equals(team)) {
                set.UnionWith(dictionary[v]);
            }
        }
        
        return set;
    }

    public void Reset () {
        dictionary.Clear();
    }
}















