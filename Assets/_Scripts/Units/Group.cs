using System;
using System.Collections.Generic;
using UnityEngine;

public class Group<T> : Group where T : Unit {

    private Vector3 _targetPosition;
    private List<T> _units;
    private bool _canMove;
    private bool _canAttack;

    // public Group(string team) {
    //     _units = new List<T>();
    //     _targetPosition = Vector3.zero;

    //     // Gets the prefab, should one exist. Throws an exception if it's not in the list.
    //     GameObject prefab = UnitManager.instance.GetPrefabOfType(typeof(T));
    //     _units.Add(UnityEngine.Object.Instantiate(prefab).GetComponent<T>());
    //     foreach (var u in _units) {
    //         u.SetTeam(team);
    //         u.SetGroup(this);
    //         TeamManager.instance.Add(team, u);
    //     }
    //     _canMove = true;
    //     _canAttack = false;
    // }

    public void Initialize (string team) {
        _units = new List<T>();
        _targetPosition = Vector3.zero;

        GameObject prefab = UnitManager.instance.GetPrefabOfType(typeof(T));

        for (int i = 0; i < prefab.GetComponent<Unit>().groupAmount; i++) {
            var unit = UnityEngine.Object.Instantiate(prefab).GetComponent<T>();
            _units.Add(unit);
            unit.Team = team;
            unit.Group = this;
            TeamManager.instance.Add(team, unit);
        }

        this.CanMove = true;
        this.CanAttack = false;
    }

    public bool CanMove
    {
        get
        {
            return _canMove;
        }
        set
        {
            _canMove = value;
            if (value == false)
            {
                foreach (var v in _units)
                {
                    v.CanMove = false;
                }
            }
            if (value == true)
            {
                foreach (var v in _units)
                {
                    v.CanMove = true;
                }
            }
        }
    }

    public bool CanAttack
    {
        get => _canAttack;
        set
        {
            _canAttack = value;
            foreach (var v in _units)
            {
                v.CanAttack = value;
            }
        }
    }

    public void TeleportTo(Vector3 position, float rotation)
    {
        foreach (var u in _units)
        {
            if (_units.Count > 1)
            {
                int i = 0;
                float angleIncrement = 2 * Mathf.PI / _units.Count;
                foreach (var v in _units)
                {
                    Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement + rotation), 0, Mathf.Sin(i * angleIncrement + rotation));
                    u.transform.position = position + offset;
                    i++;
                }
            }
            else
            {
                foreach (var v in _units)
                {
                    u.transform.position = position;
                }
            }
        }
    }

    public override void TeleportTo(Vector3 position)
    {
        TeleportTo(position, 0f);
    }

    public override void MoveTo(Vector3 position)
    {
        if (!position.Equals(_targetPosition))
        {
            _targetPosition = position;
            if (_units.Count <= 1) {
                foreach (var v in _units)
                {
                    v.IssueDestination(_targetPosition);
                }

                return;
            }

            foreach (var v in _units)
            {
                int i = 0;
                float angleIncrement = 2 * Mathf.PI / _units.Count;
                foreach (var v in _units)
                {
                    Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement), 0, Mathf.Sin(i * angleIncrement));
                    v.IssueDestination(_targetPosition + offset);
                    i++;
                }
            }
        }
    }

    public int GetLiving()
    {
        return _units.Count;
    }

    public void DestroyGroup()
    {
        // TODO: Implement
    }

    public Vector3 GetDestination()
    {
        return _targetPosition;
    }

    public string GetDescription()
    {
        // TODO: Implement
        return null;
    }

    public List<T> GetUnits()
    {
        return _units;
    }

    public override List<Unit> GetUnitsBase () {
        List<Unit> unitList = new List<Unit>(_units);

        return unitList;
    }

    internal void RemoveUnit(T unit)
    {
        _units.Remove(unit);
    }

    internal override void RemoveUnit(Unit unit)
    {
        if (unit is T) RemoveUnit((T) unit);
    }

    internal override void SetAgentEnabled (bool enabled) {
        this.CanMove = true;

        foreach (Unit u in _units) {
            u.NavMeshAgent.enabled = enabled;
        }
    }
}

public abstract class Group
{
    internal abstract void RemoveUnit(Unit unit);

    internal abstract void SetAgentEnabled(bool enabled);

    public abstract void TeleportTo(Vector3 position);

    public abstract void MoveTo(Vector3 position);

    public abstract List<Unit> GetUnitsBase();
}













