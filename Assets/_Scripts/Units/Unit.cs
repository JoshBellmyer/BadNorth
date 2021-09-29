using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Directive _directive;
    private bool _canMove;
    private bool _canAttack;
    private Vector3 _destination;

    public static float MAX_PROXIMITY
    {
        get => 5.0f;
    }

    public bool CanMove
    {
        get => _canMove;
        set
        {
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
    public bool CanAttack
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

    public void CeaseMovement()
    {
        if (_directive != Directive.NONE)
        {
            _navMeshAgent.ResetPath();
            _directive = Directive.NONE;
        }
    }
    public void IssueDestination(Vector3 destination)
    {
        // TODO: Transform destination before setting.
        _destination = destination;
        if (_canMove)
        {
            _navMeshAgent.SetDestination(destination);
            _directive = Directive.MOVE;
        }
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
}