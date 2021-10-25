using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Directive _directive;
    private string _team;
    private Group _group;
    private Vector3 _destination;

    [SerializeField] private int _health;
    private bool _canMove;
    private bool _canAttack;

    public static readonly float MAX_PROXIMITY = 5.0f;
    protected Unit(int health)
    {
        _health = health;
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

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _canAttack = false;
        _canMove = false;
        _destination = Vector3.zero;
        _directive = Directive.NONE;
    }
    void Start()
    {

    }
    void Update()
    {
        // There has got to be a better way to implement this.
        /*
        if (_navMeshAgent.isStopped)
        {
            _directive = Directive.NONE;
        }
        if (_canAttack && (_directive == Directive.NONE || _navMeshAgent.remainingDistance < MAX_PROXIMITY) && FindAttack() && !_navMeshAgent.isStopped)
        {
            _directive = Directive.ATTACK;
        }*/
    }
    private void LateUpdate()
    {
        if (_health <= 0)
        {
            _group.RemoveUnit(this);
            TeamManager.instance.Remove(_team, this);
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
        Debug.Log(_navMeshAgent is null);
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
    internal void SetGroup(Group group)
    {
        _group = group;
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
        return TeamManager.instance.GetNotOnTeam(_team);
    }
    protected HashSet<Unit> GetAllies()
    {
        return TeamManager.instance.GetOnTeam(_team);
    }
    protected IOrderedEnumerable<Unit> GetOrderedEnemiesWithin(float maximumDistance)
    {
        HashSet<Unit> enemies = GetEnemies();
        IEnumerable<Unit> e = from enemy in enemies
                              where Vector3.Distance(this.transform.position, enemy.transform.position) < maximumDistance
                              select enemy;
        IOrderedEnumerable<Unit> t = e.OrderBy(v => Vector3.Distance(this.transform.position, v.transform.position));
        return t;
    }
}