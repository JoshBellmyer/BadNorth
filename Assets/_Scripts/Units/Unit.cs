using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] protected Directive _directive;
    private string _team;
    private Group _group;
    protected Vector3 _destination;
    private LadderUnit targetLadder;
    private Vector3 secondDestination;

    private TeamColor teamColor;
    private MeshRenderer[] renderers;

    private bool climbing;
    private bool falling;
    private bool knockback;
    private bool resumeMovement;
    private Vector3 knockDestination;
    private bool paused;
    public float lastAttacked;

    [SerializeField] protected Unit _targetEnemy;
    protected float _currentCooldown;

    [SerializeField] private UnitType _unitType;
    [SerializeField] private int _health;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDistance;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] private DamageType _damageType;
    private bool _canMove;
    private bool _canAttack;
    private bool _inBoat;
    public int groupAmount;

     // public bool tempBool;

    public static readonly float MAX_PROXIMITY = 5.0f;

    internal NavMeshAgent NavMeshAgent {
        get => _navMeshAgent;
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
                    // _navMeshAgent.SetDestination(_destination);
                }
                
                // _directive = Directive.MOVE;
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
                // _navMeshAgent.SetDestination(_destination);
                // _directive = Directive.MOVE;
            }
        }
    }

    public bool InBoat {
        get => _inBoat;
        set { _inBoat = value; }
    }

    public string Team {
        get => _team;
        set { _team = value; }
    }

    public Group Group {
        get => _group;
        set { _group = value; }
    }

    public int Health {
        get => _health;
        set {_health = value;}
    }

    public UnitType Type {
        get => _unitType;
    }

    protected enum Directive
    {
        MOVE, ATTACK, NONE
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _canAttack = true;
        _canMove = true;
        _destination = Vector3.zero;
        _directive = Directive.NONE;
    }

    private void Start()
    {   
        teamColor = GetComponent<TeamColor>();
        teamColor.SetColor(int.Parse(_team));
        renderers = GetComponentsInChildren<MeshRenderer>();

        UnitStart();
    }

    protected virtual void UnitStart () {}

    private void Update()
    {
        if (InBoat) return;

        if (Game.instance.isPaused) {
            if (!paused) {
                Pause();
            }

            return;
        }
        else if (paused) {
            UnPause();
        }

        // if (_navMeshAgent.isOnNavMesh) {
        //     tempBool = _navMeshAgent.isStopped;
        // }
        // else {
        //     tempBool = false;
        // }

        // There has got to be a better way to implement this.
        float dist = Vector3.Distance(transform.position, _destination);

        if (_navMeshAgent.isOnNavMesh && (_navMeshAgent.isStopped || dist < 1f) && _directive == Directive.MOVE)
        {
            _directive = Directive.NONE;
        }
        if (_navMeshAgent.isOnNavMesh && _canAttack && (_directive == Directive.NONE || lastAttacked > 0) && FindAttack() && !_navMeshAgent.isStopped)
        {
            _directive = Directive.ATTACK;
        }

        if (lastAttacked > 0) {
            lastAttacked -= Time.deltaTime;

            if (lastAttacked <= 0) {
                lastAttacked = 0;
            }
        }

        if (!climbing) {
            CheckForGround();
        }

        if (falling) {
            FallUpdate();
        }
        else if (knockback) {
            KnockbackUpdate();
        }
        else if (targetLadder != null) {
            UpdateLadderMovement();
        }

        if (_directive == Directive.MOVE && !climbing) {
            UpdateEdgeJumping();
        }

        UnitUpdate();

        if (_directive == Directive.ATTACK) {
            AttackUpdate();
        }
    }

    protected virtual void UnitUpdate () {}

    protected virtual void AttackUpdate () {
        if (_targetEnemy == null) {
            _directive = Directive.MOVE;
            IssueDestination(_destination);

            return;
        }

        float dist = Vector3.Distance(transform.position, _targetEnemy.transform.position);

        if (dist > _attackDistance + 0.5f) {
            _targetEnemy = null;
            _directive = Directive.MOVE;
            IssueDestination(_destination);

            return;
        }

        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.SetDestination(_targetEnemy.transform.position);
        }

        if (dist > _attackRange) {
            return;
        }

        LookAt(_targetEnemy.transform.position);

        if (_currentCooldown > 0) {
            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown <= 0) {
                _currentCooldown = 0;
            }
            else {
                return;
            }
        }

        _targetEnemy.GetComponent<DamageHelper>().TakeDamage(_damageType, _targetEnemy.transform.position - transform.position);
        _currentCooldown = _attackCooldown;
    }

    protected virtual bool FindAttack () {
        HashSet<Unit> units = TeamManager.instance.GetNotOnTeam(Team);

        float minDist = _attackDistance;
        Unit target = null;

        foreach (Unit u in units) {
            float dist = Vector3.Distance(transform.position, u.transform.position);

            if (dist <= minDist) {
                minDist = dist;
                target = u;
            }
        }

        if (target == null) {
            return false;
        }

        _targetEnemy = target;
        IssueAttackLocation(target.transform.position);

        return true;
    }

    private void LateUpdate()
    {
        if (_health <= 0 || transform.position.y < Sand.height)
        {
            Die();
        }
    }

    internal void Die()
    {
        _group.RemoveUnit(this);
        TeamManager.instance.Remove(_team, this);

        DestroyMaterialInstances();

        Destroy(gameObject);
    }

    private void UpdateEdgeJumping () {
        if (!_navMeshAgent.isOnNavMesh) {
            return;
        }
        if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
            return;
        }
        if (_destination.y >= (transform.position.y - 0.25f)) {
            return;
        }

        NavMeshHit hit;
        bool foundEdge = _navMeshAgent.FindClosestEdge(out hit);

        if (hit.distance > 0.02f) {
            return;
        }

        Vector3 normal = Vector3.zero;

        if (Mathf.Abs(hit.normal.x) == 1) {
            normal = new Vector3(hit.normal.x * -1, 0, 0);
        }
        if (Mathf.Abs(hit.normal.z) == 1) {
            normal = new Vector3(0, 0, hit.normal.z * -1);
        }

        if (normal == Vector3.zero) {
            return;
        }

        Vector3 groundPos = Game.GetGroundLevelPos(transform.position + (normal * 0.5f) + new Vector3(0, 0.1f, 0));

        if (groundPos.y >= transform.position.y) {
            return;
        }

        transform.position += (normal * 0.35f);

        falling = true;
        resumeMovement = true;
        _navMeshAgent.enabled = false;
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
        }
    }

    private void FallUpdate () {
        if (_inBoat) {
            return;
        }

        transform.position += (Vector3.down * 1 * Time.deltaTime);
    }

    private void KnockbackUpdate () {
        transform.position = Vector3.MoveTowards(transform.position, knockDestination, 5 * Time.deltaTime);

        if (transform.position == knockDestination) {
            knockback = false;
            _navMeshAgent.enabled = true;
        }
    }

    internal void LookAt (Vector3 pos) {
        transform.LookAt(pos);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void SetKnockback (Vector3 knockVector) {
        knockback = true;
        _navMeshAgent.enabled = false;

        float strength = Vector3.Magnitude(knockVector);
        Vector3 direction = Vector3.Normalize(knockVector);
        direction = new Vector3(direction.x, 0, direction.z);

        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position + (Vector3.up * 0.2f), direction, out hit, strength + 0.2f, LayerMask.GetMask("Terrain"));

        if (didHit) {
            knockDestination = transform.position + (direction * (hit.distance - 0.2f));
        }
        else {
            knockDestination = transform.position + (direction * strength);
        }
    }

    public void SetColor (Color color) {
        foreach (MeshRenderer r in renderers) {
            if (r == null) {
                continue;
            }

            Material[] materials = r.materials;

            foreach (Material m in materials) {
                m.color = color;
            }

            r.materials = materials;
        }
    }

    public void SetTeamColor (int number) {
        teamColor.SetColor(number);
    }

    public void ResetTeamColor () {
        teamColor.SetColor(int.Parse(_team));
    }

    private void DestroyMaterialInstances () {
        foreach (MeshRenderer r in renderers) {
            if (r == null) {
                continue;
            }

            Material[] materials = r.materials;

            foreach (Material m in materials) {
                Destroy(m);
            }
        }
    }

    private void CheckForGround () {
        if (_inBoat) {
            return;
        }

        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.6f, LayerMask.GetMask("Terrain"));

        if (falling && didHit) {
            falling = false;
            _navMeshAgent.enabled = true;

            if (resumeMovement) {
                IssueDestination(_destination);
                resumeMovement = false;
            }
        }
        else if (!falling && !didHit) {
            falling = true;
            _navMeshAgent.enabled = false;
        }
    }

    internal void CeaseMovement () {
        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        _destination = transform.position;
        _directive = Directive.NONE;
    }

    internal void Pause () {
        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        paused = true;
    }

    internal void UnPause () {
        if (_directive == Directive.MOVE) {
            IssueDestination(_destination);
        }

        paused = false;
    }

    internal void IssueDestination(Vector3 destination)
    {
        if (!_canMove) {
            return;
        }
        if (!_navMeshAgent.isOnNavMesh) {
            return;
        }

        // TODO: Transform destination before setting.

        // _navMeshAgent.isStopped = true;
        _destination = Game.GetGroundLevelPos(destination);
        
        _navMeshAgent.SetDestination(destination);
        _directive = Directive.MOVE;

        HashSet<Unit> units = TeamManager.instance.GetOnTeam(_team);
        List<Tuple<NavMeshAgent, NavMeshAgent>> agentTuples = new List<Tuple<NavMeshAgent, NavMeshAgent>>();
        Dictionary<NavMeshAgent, LadderUnit> ladders = new Dictionary<NavMeshAgent, LadderUnit>();

        foreach (Unit u in units) {
            if (u == null) {
                continue;
            }

            if (u is LadderUnit && u != this) {
                float heightDiff = Mathf.Abs(u.transform.position.y - transform.position.y);
                float dist = Vector3.Distance(transform.position, u.transform.position);
                LadderUnit lu = (LadderUnit)u;

                if (dist <= 10 && lu.Attached) {
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

        foreach (Tuple<NavMeshAgent, NavMeshAgent> tuple in otherAgents) {
            float dist1 = UnitManager.GetRemainingDistance(tuple.Item1, minDistance);
            float dist2 = UnitManager.GetRemainingDistance(tuple.Item2, minDistance);
            float dist = dist1 + dist2;

            if (tuple.Item1.path.status != NavMeshPathStatus.PathComplete || tuple.Item2.path.status != NavMeshPathStatus.PathComplete) {
                dist += 100;
            }

            if (dist < minDistance) {
                minDistance = dist;
                pathAgent = tuple.Item1;
                dest1 = tuple.Item1.destination;
                dest2 = tuple.Item2.destination;
            }

            ClearDummyPath(tuple);
        }

        if (pathAgent != _navMeshAgent) {
            if (_navMeshAgent.isOnNavMesh) {
                pathAgent = _navMeshAgent;
            }

            _navMeshAgent.ResetPath();
            _destination = dest1;
            secondDestination = dest2;
            targetLadder = ladders[pathAgent];
            _navMeshAgent.SetDestination(dest1);
        }

        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.isStopped = false;
        }
    }

    internal Tuple<NavMeshAgent, NavMeshAgent> SetDummyPath (LadderUnit ladderUnit, Vector3 destination) {
        if (ladderUnit == null) {
            return null;
        }

        NavMeshAgent agent1 = UnitManager.instance.GetDummyAgent(transform.position);
        NavMeshAgent agent2 = UnitManager.instance.GetDummyAgent(ladderUnit.GetEdgePos());

        agent1.SetDestination(ladderUnit.GetBottomPos());
        agent2.SetDestination(destination);

        return new Tuple<NavMeshAgent, NavMeshAgent>(agent1, agent2);
    }

    internal void ClearDummyPath (Tuple<NavMeshAgent, NavMeshAgent> agents) {
        UnitManager.instance.DeactivateDummy(agents.Item1);
        UnitManager.instance.DeactivateDummy(agents.Item2);
    }

    protected virtual void OnMove () {}

    protected void IssueAttackLocation(Vector3 target)
    {
        if (_canMove)
        {
            _navMeshAgent.SetDestination(target);
            if (UnitManager.GetRemainingDistance(_navMeshAgent, MAX_PROXIMITY + 1) > MAX_PROXIMITY)
            {
                _navMeshAgent.ResetPath();
            }
        }
    }

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















