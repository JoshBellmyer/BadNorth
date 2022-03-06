using System;
using System.Collections.Generic;
using UnityEngine;

public class Group {

    private Vector3 _targetPosition;
    private List<Unit> _units;
    private bool _canMove;
    private bool _canAttack;
    private bool _inBoat;

    public static readonly float DEFAULT_RADIUS = 0.35f;


    public Group (string team, UnitType unitType) {
        _units = new List<Unit>();
        _targetPosition = Vector3.zero;

        GameObject prefab = UnitManager.instance.GetPrefabOfType(unitType);

        this.CanMove = true;
        this.CanAttack = true;

        if (Game.online) {

            return;
        }

        for (int i = 0; i < prefab.GetComponent<Unit>().groupAmount; i++) {
            Unit unit = UnityEngine.Object.Instantiate(prefab).GetComponent<Unit>();
            _units.Add(unit);
            unit.Team = team;
            unit.Group = this;
        }
    }


    public bool CanMove {
        get => _canMove;
        set {
            _canMove = value;

            foreach (Unit u in _units) {
                u.CanMove = value;
            }
        }
    }

    public bool CanAttack {
        get => _canAttack;
        set {
            _canAttack = value;

            foreach (Unit u in _units) {
                u.CanAttack = value;
            }
        }
    }

    public bool InBoat {
        get => _inBoat;
        set {
            _inBoat = value;
            
            foreach (Unit u in _units) {
                u.InBoat = value;
            }
        }
    }


    public void TeleportTo (Vector3 position, float rotation, float radius) {
        if (_units.Count > 1) {
            int i = 0;
            float angleIncrement = 2 * Mathf.PI / _units.Count;
            float rotationCorrection = angleIncrement / 2;

            foreach (Unit u in _units) {
                Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement + rotation + rotationCorrection), 0, Mathf.Sin(i * angleIncrement + rotation + rotationCorrection)) * radius;
                u.transform.position = position + offset;
                u.CeaseMovement();
                i++;
            }
        }
        else {
            _units[0].transform.position = position;
            _units[0].CeaseMovement();
        }
    }

    // ^
    public void TeleportTo (Vector3 position) {
        TeleportTo(position, 0f, DEFAULT_RADIUS);
    }


    public void MoveTo (Vector3 position, float rotation, float radius) {
        if (position.Equals(_targetPosition)) {
            return;
        }

        _targetPosition = position;

        if (_units.Count > 1) {
            int i = 0;
            float angleIncrement = 2 * Mathf.PI / _units.Count;
            float rotationCorrection = angleIncrement / 2;

            foreach (Unit u in _units) {
                Vector3 offset = new Vector3(Mathf.Cos(i * angleIncrement + rotationCorrection + rotation), 0, Mathf.Sin(i * angleIncrement + rotationCorrection + rotation)) * radius;
                u.IssueDestination(_targetPosition + offset);
                i++;
            }
        }
        else {
            _units[0].IssueDestination(_targetPosition);
        }
    }

    // ^
    public void MoveTo (Vector3 position) {
        MoveTo(position, 0f, DEFAULT_RADIUS);
    }


    public int GetLiving () {
        return _units.Count;
    }

    public void DestroyGroup () {
        foreach (Unit u in _units) {
            u.Die();
        }
    }

    public Vector3 GetDestination () {
        return _targetPosition;
    }

    public string GetDescription () {
        // TODO: Implement
        return null;
    }

    public List<Unit> GetUnits () {
        return _units;
    }

    internal void RemoveUnit(Unit unit) {
        _units.Remove(unit);

        if (_units.Count == 0) {
            DestroyGroup();
        }
    }

    internal void SetAgentEnabled (bool enabled) {
        this.CanMove = true;

        foreach (Unit u in _units) {
            u.NavMeshAgent.enabled = enabled;
        }
    }

    public void SetInBoat (bool inBoat) {
        InBoat = inBoat;
        CanMove = !inBoat;
        CanAttack = !inBoat;
    }
}















