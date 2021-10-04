using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Directive _directive;
    private bool _canMove;
    private bool _canAttack;
    private Vector3 _destination;
    private string _team;

    public static float MAX_PROXIMITY
    {
        get => 5.0f;
    }

    internal bool CanMove
    {
        get => _canMove;
        set
        {
            _canMove = value;
            if (value == true)
            {
                _navMeshAgent.SetDestination(_destination);
                _directive = Directive.MOVE;
            }
            else if (_directive != Directive.NONE)
            {
                _navMeshAgent.ResetPath();
                _directive = Directive.NONE;
            }

        }
    }

    internal bool CanAttack
    {
        get => _canAttack;
        set
        {
            _canAttack = value;
            if (value == false && _directive == Directive.ATTACK)
            {
                _navMeshAgent.SetDestination(_destination);
                _directive = Directive.MOVE;
            }
        }
    }

    enum Directive
    {
        MOVE, ATTACK, NONE
    }

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _canAttack = false;
        _destination = Vector3.zero;
        _directive = Directive.NONE;
    }
    void Update()
    {
        // There has got to be a better way to implement this.
        if (_navMeshAgent.isStopped)
        {
            _directive = Directive.NONE;
        }
        if (_canAttack && _directive == Directive.NONE && FindAttack() && !_navMeshAgent.isStopped)
        {
            _directive = Directive.ATTACK;
        }
    }

    internal void CeaseMovement()
    {
        if (_directive != Directive.NONE)
        {
            _navMeshAgent.ResetPath();
            _directive = Directive.NONE;
        }
    }
    internal void IssueDestination(Vector3 destination)
    {
        // TODO: Transform destination before setting.
        _destination = destination;
        if (_canMove)
        {
            _navMeshAgent.SetDestination(destination);
            _directive = Directive.MOVE;
        }
    }
    internal void SetTeam(string team)
    {
        _team = team;
    }
    protected void IssueAttackLocation(Vector3 target)
    {
        if (_canMove)
        {
            _navMeshAgent.SetDestination(target);
            if (_navMeshAgent.remainingDistance > MAX_PROXIMITY)
            {
                _navMeshAgent.ResetPath();
            }
        }
    }
    protected abstract bool FindAttack();
    protected HashSet<Unit> GetEnemies()
    {
        return TeamManager.Primary.GetNotOnTeam(_team);
    }
    protected HashSet<Unit> GetAllies()
    {
        return TeamManager.Primary.GetOnTeam(_team);
    }
}