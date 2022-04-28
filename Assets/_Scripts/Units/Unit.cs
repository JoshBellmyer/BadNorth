using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public abstract class Unit : MonoBehaviour {

    public float lastAttacked;
    public int groupAmount;

    [SerializeField] private UnitType _unitType;
    [SerializeField] private int _health;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDistance;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] private DamageType _damageType;
    [SerializeField] protected Sound attackSound;
    [SerializeField] protected float attackVolume;
    [SerializeField] protected Directive _directive;
    [SerializeField] protected Unit _targetEnemy;
    public NetworkUnit networkUnit;

    private LadderUnit targetLadder;
    private Vector3 secondDestination;

    private TeamColor teamColor;
    private Renderer[] renderers;

    private Animator animator;
    private string[] animationNames;
    private AnimationType currentAnimation = AnimationType.None;

    private bool climbing;
    private bool falling;
    private bool knockback;
    private bool resumeMovement;
    private Vector3 knockDestination;
    private bool paused;

    private NavMeshAgent _navMeshAgent;
    private string _team;
    private Group _group;
    private bool _canMove;
    private bool _canAttack;
    private bool _inBoat;
    protected Vector3 _destination;
    protected float _currentCooldown;
    private float startDelay = 1;

    private bool setColor;
    private List<Tuple<NavMeshAgent, NavMeshAgent>> agentTuples = new List<Tuple<NavMeshAgent, NavMeshAgent>>();

    public static readonly float MAX_PROXIMITY = 5.0f;


    internal NavMeshAgent NavMeshAgent {
        get => _navMeshAgent;
    }

    internal bool CanMove {
        get {
            if (Game.online) {
                return networkUnit.canMove.Value;
            }
            else {
                return _canMove;
            }
        }
        set {
            _canMove = value;

            if (Game.online) {
                networkUnit.SetCanMoveServerRpc(value);
            }
            
            if (_navMeshAgent.isOnNavMesh) {
                _navMeshAgent.isStopped = !value;
            }

            if (_directive != Directive.NONE && !value) {
                if (_navMeshAgent.isOnNavMesh) {
                    _navMeshAgent.ResetPath();
                }
                
                _directive = Directive.NONE;
            }
        }
    }

    internal bool CanAttack {
        get {
            if (Game.online) {
                return networkUnit.canAttack.Value;
            }
            else {
                return _canAttack;
            }
        }
        set {
            _canAttack = value;

            if (Game.online) {
                networkUnit.SetCanAttackServerRpc(value);
            }
        }
    }

    public bool InBoat {
        get {
            if (Game.online) {
                return networkUnit.inBoat.Value;
            }
            else {
                return _inBoat;
            }
        }
        set {
            if (Game.online) {
                _inBoat = value;
                networkUnit.SetInBoatServerRpc(value);
            }
            else {
                _inBoat = value;
            }
        }
    }

    public string Team {
        get {
            if (Game.online) {
                return $"{networkUnit.team.Value}";
            }
            else {
                return _team;
            }
        }
        set { 
            if (Game.online) {
                _team = value;
                networkUnit.SetTeamServerRpc(value);
            }
            else {
                _team = value; 
            }
        }
    }

    public Group Group {
        get => _group;
        set { _group = value; }
    }

    public int Health {
        get {
            if (Game.online) {
                return networkUnit.health.Value;
            }
            else {
                return _health;
            }
        }
        set {
            if (Game.online) {
                networkUnit.SetHealthServerRpc(value);
            }

            _health = value;
        }
    }

    public UnitType Type {
        get => _unitType;
    }


    protected enum Directive {
        MOVE, ATTACK, NONE
    }


    private void Awake () {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _canAttack = true;
        _canMove = true;
        _destination = Vector3.zero;
        _directive = Directive.NONE;
        
        networkUnit = GetComponent<NetworkUnit>(); 
    }

    private void Start () {
        teamColor = GetComponent<TeamColor>();
        // teamColor.SetColor(int.Parse(Team));

        if (!Game.online) {
            teamColor.SetColor(int.Parse(Team));
        }

        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null) {
            InitializeAnimations();
        }

        UnitStart();

        Health = _health;

        MusicManager.Instance.AddTheme(Type);
    }

    private void OnDestroy()
    {
        MusicManager.Instance.RemoveTheme(Type);
    }

    protected virtual void UnitStart () {}

    private void Update () {
        // Set color for online game
        if (Game.online && !setColor) {
            if (int.Parse(Team) > 0) {
                GetComponent<TeamColor>().SetColor(int.Parse(Team));
                setColor = true;
            }
        }

        if (Game.online && !Game.isHost) {
            return;
        }
        if (InBoat) {
            return;
        }

        if (Game.instance.isPaused) {
            if (!paused) {
                Pause();
            }

            return;
        }
        else if (paused) {
            UnPause();
        }

        if (!climbing) {
            CheckForGround();
        }

        if (knockback) {
            KnockbackUpdate();

            return;
        }
        else if (falling) {
            FallUpdate();

            return;
        }
        else if (targetLadder != null) {
            UpdateLadderMovement();
        }

        float dist = Vector3.Distance(transform.position, _destination);
        CanAttack = (_navMeshAgent.isOnNavMesh && (_navMeshAgent.isStopped || dist < 1f || lastAttacked > 0) && _directive != Directive.ATTACK);

        if (_navMeshAgent.isOnNavMesh && (_navMeshAgent.isStopped || dist < 0.1f) && _directive == Directive.MOVE) {
            SetAnimation(AnimationType.Idle);
            _directive = Directive.NONE;
        }

        if (_navMeshAgent.isOnNavMesh && CanAttack && FindAttack() && !_navMeshAgent.isStopped) {
            _directive = Directive.ATTACK;
        }

        if (lastAttacked > 0) {
            lastAttacked -= Time.deltaTime;

            if (lastAttacked <= 0) {
                lastAttacked = 0;
            }
        }

        if (_directive != Directive.NONE && !climbing) {
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

        if (dist > _attackRange) {
            if (_navMeshAgent.isOnNavMesh) {
                _navMeshAgent.SetDestination(_targetEnemy.transform.position);
            }

            SetAnimation(AnimationType.Walk);

            return;
        }

        LookAt(_targetEnemy.transform.position);

        if (_currentCooldown > 0) {
            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown <= 0) {
                _currentCooldown = 0;
            }
            else {
                SetAnimation(AnimationType.Idle);

                return;
            }
        }
    
        SoundPlayer.PlaySound(attackSound, attackVolume, true);
        SetAnimation(AnimationType.Attack);

        _targetEnemy.GetComponent<DamageHelper>().TakeDamage(_damageType, _targetEnemy.transform.position - transform.position);
        _currentCooldown = _attackCooldown;
    }

    protected virtual bool FindAttack () {
        HashSet<Unit> units = TeamManager.instance.GetNotOnTeam(Team);

        // Debug.Log(units.Count);

        float minDist = _attackDistance;
        Unit target = null;

        foreach (Unit u in units) {
            if (u == null) continue;
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

    private void LateUpdate () {
        if (startDelay > 0) {
            startDelay -= Time.deltaTime;

            return;
        }

        if (Health <= 0 || (transform.position.y < Sand.height && transform.parent == null)) {
            Die();
        }
    }

    internal void Die () {
        SoundPlayer.PlaySound(Sound.Death, 1.0f, true);

        if (climbing && targetLadder != null) {
            targetLadder.Occupied = false;
        }

        if (_group != null) {
            _group.RemoveUnit(this);
        }

        TeamManager.instance.Remove(Team, this);

        ClearAllAgents();
        DestroyMaterialInstances();

        if (!Game.online || Game.isHost) {
            Destroy(gameObject);
        }
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

        Vector3 groundPos = GridUtils.GetGroundLevelPos(transform.position + (normal * 0.5f) + new Vector3(0, 0.1f, 0));

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
        if (InBoat) {
            return;
        }

        // Debug.Log("fall");

        transform.position += (Vector3.down * 1 * Time.deltaTime);
    }

    private void KnockbackUpdate () {
        transform.position = Vector3.MoveTowards(transform.position, knockDestination, 5 * Time.deltaTime);

        if (transform.position == knockDestination) {
            knockback = false;
            _navMeshAgent.enabled = true;

            CheckForGround();
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
        foreach (Renderer r in renderers) {
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
        teamColor.SetColor(int.Parse(Team));
    }

    public void SetAgentEnabled (bool enabled) {
        if (Game.online && !Game.isHost) {
            networkUnit.SetAgentEnabledServerRpc(enabled);
        }
        else {
            NavMeshAgent.enabled = enabled;
        }
    }

    private void DestroyMaterialInstances () {
        foreach (Renderer r in renderers) {
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
        if (InBoat) {
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

            if (_navMeshAgent.isOnNavMesh) {
                _navMeshAgent.ResetPath();
            }
            
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
        if (animator != null) {
            animator.enabled = false;
        }

        paused = true;
    }

    internal void UnPause () {
        if (_directive == Directive.MOVE) {
            IssueDestination(_destination);
        }
        if (animator != null) {
            animator.enabled = true;
        }

        paused = false;
    }

    public void IssueDestination (Vector3 destination) {
        if (Game.online && !Game.isHost) {
            networkUnit.IssueDestinationServerRpc(destination);

            return;
        }

        // Debug.Log(_navMeshAgent.isOnNavMesh);

        if (!CanMove) {
            return;
        }
        if (!_navMeshAgent.isOnNavMesh) {
            return;
        }

        SetAnimation(AnimationType.Walk);

        // TODO: Transform destination before setting.

        _destination = GridUtils.GetGroundLevelPos(destination);
        
        _navMeshAgent.SetDestination(destination);
        _directive = Directive.MOVE;

        HashSet<Unit> units = TeamManager.instance.GetOnTeam(Team);
        // List<Tuple<NavMeshAgent, NavMeshAgent>> agentTuples = new List<Tuple<NavMeshAgent, NavMeshAgent>>();
        ClearAllAgents();
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
                _navMeshAgent.ResetPath();
                _destination = dest1;
                secondDestination = dest2;
                targetLadder = ladders[pathAgent];
                _navMeshAgent.SetDestination(dest1);
            }
            else {
                pathAgent = _navMeshAgent;
            } 
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

        if (agent1 == null || agent2 == null) {
            UnitManager.instance.DeactivateDummy(agent1);
            UnitManager.instance.DeactivateDummy(agent2);

            return null;
        }

        agent1.SetDestination(ladderUnit.GetBottomPos());
        agent2.SetDestination(destination);

        return new Tuple<NavMeshAgent, NavMeshAgent>(agent1, agent2);
    }

    internal void ClearDummyPath (Tuple<NavMeshAgent, NavMeshAgent> agents) {
        UnitManager.instance.DeactivateDummy(agents.Item1);
        UnitManager.instance.DeactivateDummy(agents.Item2);
    }

    internal void ClearAllAgents () {
        foreach (Tuple<NavMeshAgent, NavMeshAgent> agents in agentTuples) {
            if (agents != null) {
                UnitManager.instance.DeactivateDummy(agents.Item1);
                UnitManager.instance.DeactivateDummy(agents.Item2);
            }
        }

        agentTuples.Clear();
    }

    protected virtual void OnMove () {}

    protected void IssueAttackLocation(Vector3 target) {
        if (CanMove) {
            _navMeshAgent.SetDestination(target);

            if (UnitManager.GetRemainingDistance(_navMeshAgent, MAX_PROXIMITY + 1) > MAX_PROXIMITY) {
                _navMeshAgent.ResetPath();
            }
        }
    }

    protected HashSet<Unit> GetEnemies () {
        return TeamManager.instance.GetNotOnTeam(Team);
    }

    protected HashSet<Unit> GetAllies () {
        return TeamManager.instance.GetOnTeam(Team);
    }

    protected IOrderedEnumerable<Unit> GetOrderedEnemiesWithin (float maximumDistance) {
        HashSet<Unit> enemies = GetEnemies();
        IEnumerable<Unit> e = from enemy in enemies
                              where Vector3.Distance(this.transform.position, enemy.transform.position) < maximumDistance
                              select enemy;
        IOrderedEnumerable<Unit> t = e.OrderBy(v => Vector3.Distance(this.transform.position, v.transform.position));
        
        return t;
    }

    public void SetAnimation (AnimationType animation) {
        if (animator == null) {
            return;
        }
        if (currentAnimation == animation) {
            return;
        }

        if (Game.online && Game.isHost) {
            networkUnit.SetAnimationClientRpc(animation);
        }

        foreach (string str in animationNames) {
            bool newValue = (str == $"{animation}");

            animator.SetBool(str, newValue);
        }

        currentAnimation = animation;
    }

    protected void InitializeAnimations () {
        AnimatorControllerParameter[] parameters = animator.parameters;
        animationNames = new string[parameters.Length];
        int index = 0;

        foreach (AnimatorControllerParameter acp in parameters) {
            animationNames[index] = parameters[index].name;
            index++;
        }
    }
}


public enum AnimationType {
    None = -1,
    Idle = 0,
    Walk = 1,
    Attack = 2,
}















