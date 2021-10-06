using System;
using System.Collections.Generic;
using UnityEngine;
public class Group<T> : Group where T : Unit {

    private Vector3 _targetPosition;
    private List<T> _units;
    private bool _canMove;
    private bool _canAttack;

    public Group(string team) {
        _units = new List<T>();
        _targetPosition = Vector3.zero;
        _canMove = false;
        _canAttack = false;

        // Gets the prefab, should one exist. Throws an exception if it's not in the list.
        GameObject prefab = UnitManager.instance.GetPrefabOfType(typeof(T));
        _units.Add(UnityEngine.Object.Instantiate(prefab).GetComponent<T>());

        foreach (var u in _units) {
            u.SetTeam(team);
            u.SetGroup(this);
            TeamManager.instance.Add(team, u);
        }
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
        // TODO: Implement
    }
    public void TeleportTo(Vector3 position)
    {
        TeleportTo(position, 0f);
    }
    public void MoveTo(Vector3 position)
    {
        _targetPosition = position;
        if (_canMove && !position.Equals(_targetPosition))
        {
            foreach (var v in _units)
            {
                v.IssueDestination(_targetPosition);
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
    internal void RemoveUnit(T unit)
    {
        _units.Remove(unit);
    }

    internal override void RemoveUnit(Unit unit)
    {
        if (unit is T) RemoveUnit((T) unit);
    }
}
public abstract class Group
{
    internal abstract void RemoveUnit(Unit unit);
}