using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager
{
    public static TeamManager Primary = new TeamManager();


    private Dictionary<string, HashSet<Unit>> dictionary = new Dictionary<string, HashSet<Unit>>();

    public TeamManager()
    {

    }

    public void Add(string team, Unit unit)
    {
        if (!dictionary.ContainsKey(team))
        {
            dictionary.Add(team, new HashSet<Unit>());
        }
        dictionary[team].Add(unit);
    }
    public void Remove(string team, Unit unit)
    {
        dictionary[team].Remove(unit);
    }

    public HashSet<Unit> GetOnTeam(string team)
    {
        HashSet<Unit> set = new HashSet<Unit>();
        set.UnionWith(dictionary[team]);
        return set;
    }
    public HashSet<Unit> GetNotOnTeam(string team)
    {
        HashSet<Unit> set = new HashSet<Unit>();
        foreach (var v in dictionary.Keys) {
            if (!v.Equals(team))
            {
                set.UnionWith(dictionary[v]);
            }
        }
        return set;
    }
    public void Reset()
    {
        dictionary.Clear();
    }
}
