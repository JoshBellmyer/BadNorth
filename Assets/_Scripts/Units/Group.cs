using System;
using System.Collections.Generic;
using UnityEngine;

public class Group<T> : Group where T : Unit {

    private Vector3 _targetPosition;
    private List<T> _units;
    private bool _canMove;
    private bool _canAttack;
    private bool _inBoat;

    public static readonly float DEFAULT_RADIUS = 0.35f;
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
        }

        this.CanMove = true;
        this.CanAttack = true;
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

    public bool InBoat {
        get => _inBoat;
        set {
            _inBoat = value;
            
            foreach (var v in _units) {
                v.InBoat = value;
            }
        }
    }

    public void TeleportTo(Vector3 position, float rotation, float radius)
    {
        if (_units.Count > 1)
        {
            int i = 0;
            float angleIncrement = 2 * Mathf.PI / _units.Count;
            float rotationCorrection = angleIncrement / 2;
            foreach (var v in _units)
            {
                Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement + rotation + rotationCorrection), 0, Mathf.Sin(i * angleIncrement + rotation + rotationCorrection)) * radius;
                v.transform.position = position + offset;
                v.CeaseMovement();
                i++;
            }
        }
        else
        {
            foreach (var v in _units)
            {
                v.transform.position = position;
                v.CeaseMovement();
            }
        }
    }

    public override void TeleportTo(Vector3 position)
    {
        TeleportTo(position, 0f, DEFAULT_RADIUS);
    }

    public override void MoveTo(Vector3 position)
    {
        MoveTo(position, 0f, DEFAULT_RADIUS);
    }
    public void MoveTo(Vector3 position, float rotation, float radius)
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

            int i = 0;
            float angleIncrement = 2 * Mathf.PI / _units.Count;
            float rotationCorrection = angleIncrement / 2;

            foreach (var v in _units)
            {
                Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement + rotationCorrection + rotation), 0, Mathf.Sin(i * angleIncrement + rotationCorrection + rotation)) * radius;
                v.IssueDestination(_targetPosition + offset);
                i++;
            }
        }
    }

    public override int GetLiving()
    {
        return _units.Count;
    }

    public void DestroyGroup()
    {
        foreach (var v in _units)
        {
            v.Die();
        }
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
        if (_units.Count == 0) DestroyGroup();
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

    public override void SetInBoat (bool inBoat) {
        InBoat = inBoat;
        CanMove = !inBoat;
        CanAttack = !inBoat;
    }
}

public abstract class Group
{
    internal abstract void RemoveUnit(Unit unit);

    internal abstract void SetAgentEnabled(bool enabled);

    public abstract void TeleportTo(Vector3 position);

    public abstract void MoveTo(Vector3 position);

    public abstract List<Unit> GetUnitsBase();

    public abstract void SetInBoat(bool inBoat);

    public abstract int GetLiving();
}













