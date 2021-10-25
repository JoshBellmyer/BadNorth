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
    protected Vector3 _destination;
    private LadderUnit targetLadder;
    private Vector3 secondDestination;
    private bool climbing;

    [SerializeField] private int _health;
    private bool _canMove;
    private bool _canAttack;
    public int groupAmount;

    public static readonly float MAX_PROXIMITY = 5.0f;

    internal NavMeshAgent NavMeshAgent {
        get => _navMeshAgent;
    }


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
            
            if (_navMeshAgent.isOnNavMesh) {
                _navMeshAgent.isStopped = !value;
            }

            if (value == true)
            {
                if (_navMeshAgent.isOnNavMesh) {
                    _navMeshAgent.SetDestination(_destination);
                }
                
                _directive = Directive.MOVE;
            }
            else if (_directive != Directive.NONE)
            {
                if (_navMeshAgent.isOnNavMesh) {
                    _navMeshAgent.ResetPath();
                }
                
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

    public string Team {
        get => _team;
        set { _team = value; }
    }

    public Group Group {
        get => _group;
        set { _group = value; }
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

    private void Start()
    {

    }

    private void Update()
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

        if (targetLadder != null) {
            UpdateLadderMovement();
        }

        UnitUpdate();
    }

    protected virtual void UnitUpdate () {}

    private void LateUpdate()
    {
        if (_health <= 0)
        {
            _group.RemoveUnit(this);
            TeamManager.instance.Remove(_team, this);
        }
    }

    private void UpdateLadderMovement () {
        if (climbing) {
            transform.position = Vector3.MoveTowards(transform.position, targetLadder.GetEdgePos(), 0.75f * Time.deltaTime);

            if (transform.position == targetLadder.GetEdgePos()) {
                transform.position = targetLadder.GetEdgePos();
                climbing = false;
                _navMeshAgent.enabled = true;
                targetLadder.Occupied = false;
                targetLadder = null;
                IssueDestination(secondDestination);
            }

            return;
        }

        if (!targetLadder.Attached) {
            targetLadder = null;
            IssueDestination(secondDestination);

            return;
        }

        float ladderDist = Vector3.Distance(transform.position, targetLadder.GetBottomPos());

        if (ladderDist < 0.3f && !targetLadder.Occupied) {
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
            climbing = true;
            targetLadder.Occupied = true;
            // transform.position = targetLadder.GetEdgePos();
            // _navMeshAgent.enabled = true;
            // targetLadder = null;
            // IssueDestination(secondDestination);
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
        if (!_canMove) {
            return;
        }

        // TODO: Transform destination before setting.

        _navMeshAgent.isStopped = true;
        _destination = Game.GetGroundLevelPos(destination);
        
        _navMeshAgent.SetDestination(destination);
        _directive = Directive.MOVE;

        HashSet<Unit> units = TeamManager.instance.GetOnTeam(_team);
        List<Tuple<NavMeshAgent, NavMeshAgent>> agentTuples = new List<Tuple<NavMeshAgent, NavMeshAgent>>();
        Dictionary<NavMeshAgent, LadderUnit> ladders = new Dictionary<NavMeshAgent, LadderUnit>();

        foreach (Unit u in units) {
            if (u is LadderUnit && u != this) {
                float heightDiff = Mathf.Abs(u.transform.position.y - transform.position.y);
                float dist = Vector3.Distance(transform.position, u.transform.position);
                LadderUnit lu = (LadderUnit)u;

                if (heightDiff <= 0.5f && dist <= 6 && lu.Attached) {
                    Tuple<NavMeshAgent, NavMeshAgent> tuple = SetDummyPath(u as LadderUnit, destination);
                    agentTuples.Add(tuple);
                    ladders.Add(tuple.Item1, lu);
                }
            }
        }

        if (agentTuples.Count > 0) {
            StartCoroutine( ChooseDestination(agentTuples, ladders) );
        }
        else {
            _navMeshAgent.isStopped = false;
        }

        OnMove();
    }

    internal IEnumerator<bool> ChooseDestination (List<Tuple<NavMeshAgent, NavMeshAgent>> otherAgents, Dictionary<NavMeshAgent, LadderUnit> ladders) {
        bool foundPaths = false;

        while (!foundPaths) {
            foundPaths = !_navMeshAgent.pathPending;

            foreach (Tuple<NavMeshAgent, NavMeshAgent> tuple in otherAgents) {
                bool foundPath = (!tuple.Item1.pathPending && !tuple.Item2.pathPending);
                foundPaths = foundPaths && foundPath;
            }

            yield return false;
        }

        float minDistance = UnitManager.GetRemainingDistance(_navMeshAgent, 1000);
        NavMeshAgent pathAgent = _navMeshAgent;
        Vector3 dest1 = _destination;
        Vector3 dest2 = _destination;

        if (_navMeshAgent.path.status != NavMeshPathStatus.PathComplete) {
            minDistance += 100;
        }

        // Debug.Log($"min: {minDistance}");

        foreach (Tuple<NavMeshAgent, NavMeshAgent> tuple in otherAgents) {
            // float dist = tuple.Item1.remainingDistance + tuple.Item2.remainingDistance;
            float dist1 = UnitManager.GetRemainingDistance(tuple.Item1, minDistance);
            float dist2 = UnitManager.GetRemainingDistance(tuple.Item2, minDistance);
            // float dist = UnitManager.GetRemainingDistance(tuple.Item1, minDistance) + UnitManager.GetRemainingDistance(tuple.Item2, minDistance);
            float dist = dist1 + dist2;

            if (tuple.Item1.path.status != NavMeshPathStatus.PathComplete || tuple.Item2.path.status != NavMeshPathStatus.PathComplete) {
                dist += 100;
            }

            // Debug.Log(tuple.Item1.transform.position);
            // Debug.Log($"first: {dist1}  second: {dist2}");

            if (dist < minDistance) {
                minDistance = dist;
                pathAgent = tuple.Item1;
                dest1 = tuple.Item1.destination;
                dest2 = tuple.Item2.destination;
            }

            ClearDummyPath(tuple);
        }

        if (pathAgent != _navMeshAgent) {
            // Debug.Log(pathAgent);
            _navMeshAgent.ResetPath();
            _destination = dest1;
            secondDestination = dest2;
            targetLadder = ladders[pathAgent];
            _navMeshAgent.SetDestination(dest1);
        }

        _navMeshAgent.isStopped = false;
    }

    internal Tuple<NavMeshAgent, NavMeshAgent> SetDummyPath (LadderUnit ladderUnit, Vector3 destination) {
        NavMeshAgent agent1 = UnitManager.GetDummyAgent(transform.position);
        NavMeshAgent agent2 = UnitManager.GetDummyAgent(ladderUnit.GetEdgePos());

        // agent1.transform.position = transform.position;
        agent1.SetDestination(ladderUnit.GetBottomPos());

        // agent2.transform.position = ladderUnit.GetEdgePos();
        agent2.SetDestination(destination);

        return new Tuple<NavMeshAgent, NavMeshAgent>(agent1, agent2);
    }

    internal void ClearDummyPath (Tuple<NavMeshAgent, NavMeshAgent> agents) {
        UnitManager.DeactivateDummy(agents.Item1);
        UnitManager.DeactivateDummy(agents.Item2);
    }

    protected virtual void OnMove () {}

    // internal void SetTeam(string team)
    // {
    //     _team = team;
    // }

    // internal void SetGroup(Group group)
    // {
    //     _group = group;
    // }

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















