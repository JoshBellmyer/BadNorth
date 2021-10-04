using System;
using System.Collections.Generic;
using UnityEngine;

public class Group<T> where T : Unit {

    private Vector3 _targetPosition;
    private List<T> _units;
    private bool _canMove;
    private bool _canAttack;

    public Group(string team) {
        _units = new List<T>();
        _targetPosition = Vector3.zero;
        _canMove = false;
        _canAttack = false;
        _units.Add((T)Activator.CreateInstance(typeof(T)));
        // _units.Add((T) Activator.CreateInstance(typeof(T)));
        // _units.Add((T) Activator.CreateInstance(typeof(T)));
        // _units.Add((T) Activator.CreateInstance(typeof(T)));
        foreach (var u in _units) {
            u.SetTeam(team);
            TeamManager.Primary.Add(team, u);
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
}
