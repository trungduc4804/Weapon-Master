using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : EnemyBase
{
    public enum AIState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Pathfinding")]
    [SerializeField] private Grid2D roomGrid;
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float waypointReachDistance = 0.12f;
    [SerializeField] private float playerReacquireInterval = 1f;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] private string walkParameter = "isWalk";
    [SerializeField] private string attackTrigger = "isAttack";
    [SerializeField] private bool useAnimationEventDamage = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugPath = false;
    [SerializeField] private Color debugPathColor = Color.cyan;

    public AIState CurrentState
    {
        get { return currentState; }
    }

    private static Player cachedPlayer;

    private readonly List<Vector3> currentPath = new List<Vector3>(64);
    private Pathfinding pathfinding;
    private AIState currentState = AIState.Idle;
    private int currentPathIndex;
    private float nextRepathTime;
    private float nextPlayerLookupTime;
    private float waypointReachDistanceSqr;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            enabled = false;
            return;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.fireEvents = useAnimationEventDamage;
        }

        pathfinding = new Pathfinding();
        waypointReachDistanceSqr = waypointReachDistance * waypointReachDistance;

        if (cachedPlayer == null)
        {
            cachedPlayer = FindFirstObjectByType<Player>();
        }

        player = cachedPlayer;
        if (room == null)
        {
            room = GetComponentInParent<Room>();
        }

        CacheGridFromRoom();
    }

    protected override void Update()
    {
        if (isDead || !canMove)
        {
            return;
        }

        if (!TryEnsurePlayerReference())
        {
            SetIdle();
            return;
        }

        if (player.health <= 0f)
        {
            SetIdle();
            return;
        }

        if (room != null && player.CurrentRoom != room)
        {
            SetIdle();
            return;
        }

        Vector2 delta = player.transform.position - transform.position;
        float distanceSqr = delta.sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;
        float detectionRangeSqr = detectionRange * detectionRange;

        if (distanceSqr <= attackRangeSqr)
        {
            ChangeState(AIState.Attack);
            isChasing = true;
        }
        else if (distanceSqr <= detectionRangeSqr)
        {
            ChangeState(AIState.Chase);
            isChasing = true;
        }
        else
        {
            ChangeState(AIState.Idle);
            isChasing = false;
        }

        switch (currentState)
        {
            case AIState.Idle:
                HandleIdleState();
                break;

            case AIState.Chase:
                HandleChaseState();
                break;

            case AIState.Attack:
                HandleAttackState();
                break;
        }
    }

    public override void SetRoom(Room newRoom)
    {
        base.SetRoom(newRoom);
        roomGrid = null;
        CacheGridFromRoom();
    }

    private void HandleIdleState()
    {
        currentPath.Clear();
        currentPathIndex = 0;
        StopMoving();
    }

    private void HandleChaseState()
    {
        if (roomGrid == null)
        {
            CacheGridFromRoom();
            if (roomGrid == null)
            {
                MoveDirectlyTowardsPlayer();
                return;
            }
        }

        if (Time.time >= nextRepathTime)
        {
            nextRepathTime = Time.time + repathInterval;
            RecalculatePath();
        }

        MoveAlongPath();

        if (currentPath.Count == 0)
        {
            MoveDirectlyTowardsPlayer();
        }
    }

    private void HandleAttackState()
    {
        StopMoving();

        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlaySFXAtPoint(AudioManager.Instance.CueLibrary.EnemyAttack, transform.position);
        }

        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        if (!useAnimationEventDamage)
        {
            DealDamage();
        }
    }

    // Optional animation event hook. If not using animation events,
    // damage is applied directly from HandleAttackState.
    public void DealDamage()
    {
        if (player == null || player.health <= 0f)
        {
            return;
        }

        if (room != null && player.CurrentRoom != room)
        {
            return;
        }

        Vector2 delta = player.transform.position - transform.position;
        if (delta.sqrMagnitude <= attackRange * attackRange)
        {
            player.TakeDamage(damage);
        }
    }

    private void RecalculatePath()
    {
        if (player == null || roomGrid == null)
        {
            currentPath.Clear();
            currentPathIndex = 0;
            return;
        }

        bool foundPath = pathfinding.FindPath(
            roomGrid,
            transform.position,
            player.transform.position,
            currentPath
        );

        if (!foundPath)
        {
            currentPath.Clear();
        }

        currentPathIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            StopMoving();
            return;
        }

        Vector2 currentPosition = rb.position;

        while (currentPathIndex < currentPath.Count)
        {
            Vector2 targetNodePos = currentPath[currentPathIndex];
            Vector2 toTarget = targetNodePos - currentPosition;

            if (toTarget.sqrMagnitude > waypointReachDistanceSqr)
            {
                Vector2 moveDir = toTarget.normalized;
                rb.linearVelocity = moveDir * speedEnemy;
                Flip(moveDir.x);
                SetWalkAnimation(true);
                return;
            }

            currentPathIndex++;
        }

        StopMoving();
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        SetWalkAnimation(false);
    }

    private void MoveDirectlyTowardsPlayer()
    {
        if (player == null || rb == null)
        {
            StopMoving();
            return;
        }

        Vector2 toPlayer = (Vector2)(player.transform.position - transform.position);
        float attackRangeSqr = attackRange * attackRange;
        if (toPlayer.sqrMagnitude <= attackRangeSqr)
        {
            StopMoving();
            return;
        }

        Vector2 moveDir = toPlayer.normalized;
        rb.linearVelocity = moveDir * speedEnemy;
        Flip(moveDir.x);
        SetWalkAnimation(true);
    }

    private void SetIdle()
    {
        ChangeState(AIState.Idle);
        HandleIdleState();
    }

    private void ChangeState(AIState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        if (newState != AIState.Chase)
        {
            SetWalkAnimation(false);
        }
    }

    private void SetWalkAnimation(bool isWalking)
    {
        if (animator == null || string.IsNullOrEmpty(walkParameter))
        {
            return;
        }

        animator.SetBool(walkParameter, isWalking);
    }

    private void CacheGridFromRoom()
    {
        if (roomGrid != null)
        {
            return;
        }

        if (room == null)
        {
            return;
        }

        roomGrid = room.GetComponentInChildren<Grid2D>();
    }

    private bool TryEnsurePlayerReference()
    {
        if (player != null)
        {
            return true;
        }

        if (Time.time < nextPlayerLookupTime)
        {
            return false;
        }

        nextPlayerLookupTime = Time.time + playerReacquireInterval;

        if (cachedPlayer == null)
        {
            cachedPlayer = FindFirstObjectByType<Player>();
        }

        player = cachedPlayer;
        return player != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugPath || currentPath == null || currentPath.Count == 0)
        {
            return;
        }

        Gizmos.color = debugPathColor;

        Vector3 from = transform.position;
        for (int i = currentPathIndex; i < currentPath.Count; i++)
        {
            Vector3 to = currentPath[i];
            Gizmos.DrawLine(from, to);
            Gizmos.DrawSphere(to, 0.07f);
            from = to;
        }
    }
}
