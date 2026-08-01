using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Monster_Controller : MonoBehaviour
{
    [Header("NavMesh Target")]
    [Tooltip("ScriptableObject containing the monster's current target position.")]
    public Vector3List target;

    [Tooltip("The NavMeshAgent used to move the monster.")]
    public NavMeshAgent agent;

    [Tooltip("How far Unity searches for a valid nearby NavMesh position.")]
    public float navMeshSampleDistance = 3f;

    [Header("Roaming Settings")]
    [Tooltip("The position and scale used to determine the roaming area.")]
    public Transform detectionRange;

    [Header("Player Detection")]
    [Tooltip("The player that the monster detects and chases.")]
    public Transform player;

    [Tooltip("How close the player must be before the monster begins chasing.")]
    public float playerDetectionDistance = 8f;

    [Tooltip("How far away the player must get before the monster stops chasing.")]
    public float losePlayerDistance = 12f;

    [Header("Player Catching")]
    [Tooltip("How close the monster must be to catch the player.")]
    public float catchDistance = 1.5f;

    [Tooltip("Invoked once when the monster catches the player.")]
    public UnityEvent onPlayerCaught;

    [Header("Item Tracking")]
    [Tooltip("The item that the monster is currently tracking.")]
    public Transform trackedItem;

    [Header("Animation")]
    [Tooltip("The Animator controlling the monster animations.")]
    public Animator animator;

    [Tooltip("The Animator state played while roaming.")]
    public string roamingAnimationParameter = "isWalking";

    [Tooltip("The Animator state played while chasing or tracking an item.")]
    public string chasingAnimationParameter = "isChasing";

    [Header("State Events")]
    [Tooltip("Invoked once when the monster begins roaming.")]
    public UnityEvent onRoaming;

    [Tooltip("Invoked once when the monster begins chasing.")]
    public UnityEvent onChasing;

    [Tooltip("Invoked once when the monster begins tracking an item.")]
    public UnityEvent onTrackingItem;

    [Header("Monster State")]
    [Tooltip("The monster's current behavior state.")]
    public MonsterState currentState = MonsterState.Roaming;

    public enum MonsterState
    {
        Roaming,
        Chasing,
        TrackingItem
    }

    private MonsterState previousState;
    private bool stateInitialized;
    private bool hasCaughtPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!CheckRequiredReferences())
        {
            enabled = false;
            return;
        }

        ChangeState(MonsterState.Roaming);
    }

    private void Update()
    {
        CheckForPlayer();

        // Allows the state to be manually changed in the Inspector
        // while the game is running.
        if (!stateInitialized || currentState != previousState)
        {
            EnterState(currentState);

            previousState = currentState;
            stateInitialized = true;
        }

        if (hasCaughtPlayer)
        {
            return;
        }

        switch (currentState)
        {
            case MonsterState.Roaming:
                RoamingUpdate();
                break;

            case MonsterState.Chasing:
                ChasingUpdate();
                break;

            case MonsterState.TrackingItem:
                TrackingItemUpdate();
                break;
        }
    }

    private bool CheckRequiredReferences()
    {
        if (target == null)
        {
            Debug.LogError(
                $"{name}: No Vector3List target has been assigned.",
                this);

            return false;
        }

        if (target.value == null || target.value.Length == 0)
        {
            Debug.LogError(
                $"{name}: The assigned Vector3List must contain at least one position.",
                this);

            return false;
        }

        if (detectionRange == null)
        {
            Debug.LogError(
                $"{name}: No roaming detection range has been assigned.",
                this);

            return false;
        }

        return true;
    }

    #region Player Detection

    private void CheckForPlayer()
    {
        if (player == null || hasCaughtPlayer)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position);

        if (currentState == MonsterState.Roaming &&
            distanceToPlayer <= playerDetectionDistance)
        {
            StartChasingPlayer();
        }
        else if (currentState == MonsterState.Chasing &&
                 distanceToPlayer >= losePlayerDistance)
        {
            StartRoaming();
        }
    }

    #endregion

    #region Public State Methods

    /// <summary>
    /// Makes the monster begin roaming.
    /// This can be called through a UnityEvent.
    /// </summary>
    public void StartRoaming()
    {
        hasCaughtPlayer = false;
        ChangeState(MonsterState.Roaming);
    }

    /// <summary>
    /// Makes the monster begin chasing the assigned player.
    /// This can be called through a UnityEvent.
    /// </summary>
    public void StartChasingPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(
                $"{name}: Cannot chase because no player is assigned.",
                this);

            return;
        }

        hasCaughtPlayer = false;
        ChangeState(MonsterState.Chasing);
    }

    /// <summary>
    /// Stops chasing and returns to roaming.
    /// </summary>
    public void StopChasingPlayer()
    {
        StartRoaming();
    }

    /// <summary>
    /// Begins tracking the item assigned in the Inspector.
    /// This can be called through a parameterless UnityEvent.
    /// </summary>
    public void StartTrackingAssignedItem()
    {
        if (trackedItem == null)
        {
            Debug.LogWarning(
                $"{name}: Cannot track because no item is assigned.",
                this);

            return;
        }

        hasCaughtPlayer = false;
        ChangeState(MonsterState.TrackingItem);
    }

    /// <summary>
    /// Assigns an item and immediately begins tracking it.
    /// This is intended to be called by another script.
    /// </summary>
    public void StartTrackingItem(Transform item)
    {
        if (item == null)
        {
            Debug.LogWarning(
                $"{name}: Cannot track a null item.",
                this);

            return;
        }

        trackedItem = item;
        hasCaughtPlayer = false;

        ChangeState(MonsterState.TrackingItem);
    }

    /// <summary>
    /// Assigns the player's Transform.
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    /// <summary>
    /// Assigns an item without immediately tracking it.
    /// </summary>
    public void SetTrackedItem(Transform newItem)
    {
        trackedItem = newItem;
    }

    #endregion

    #region State Management

    private void ChangeState(MonsterState newState)
    {
        currentState = newState;
        previousState = newState;
        stateInitialized = true;

        EnterState(newState);
    }

    private void EnterState(MonsterState newState)
    {
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        switch (newState)
        {
            case MonsterState.Roaming:
                onRoaming?.Invoke();
                PlayAnimation(roamingAnimationParameter);
                SetRandomRoamingPosition();
                break;

            case MonsterState.Chasing:
                onChasing?.Invoke();
                PlayAnimation(chasingAnimationParameter);
                break;

            case MonsterState.TrackingItem:
                onTrackingItem?.Invoke();
                PlayAnimation(chasingAnimationParameter);
                break;
        }
    }

    #endregion

    #region Roaming

    private void RoamingUpdate()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            SetRandomRoamingPosition();
        }

        DrawAgentPath(Color.red);
    }

    private void SetRandomRoamingPosition()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                $"{name}: The monster is not standing on a NavMesh.",
                this);

            return;
        }

        float roamingRadius =
            Mathf.Abs(detectionRange.localScale.x) * 0.5f;

        // Attempts multiple positions in case a random point
        // is outside the walkable NavMesh.
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomPoint =
                Random.insideUnitCircle * roamingRadius;

            Vector3 desiredPosition =
                detectionRange.position +
                new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (NavMesh.SamplePosition(
                    desiredPosition,
                    out NavMeshHit hit,
                    navMeshSampleDistance,
                    agent.areaMask))
            {
                target.value[0] = hit.position;
                agent.SetDestination(hit.position);
                return;
            }
        }

        Debug.LogWarning(
            $"{name}: Could not find a valid roaming position.",
            this);
    }

    #endregion

    #region Chasing

    private void ChasingUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning(
                $"{name}: The player reference is missing.",
                this);

            StartRoaming();
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        SetDestinationNearPosition(player.position);

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position);

        if (distanceToPlayer <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        DrawAgentPath(Color.green);
    }

    private void CatchPlayer()
    {
        if (hasCaughtPlayer)
        {
            return;
        }

        hasCaughtPlayer = true;

        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        onPlayerCaught?.Invoke();
    }

    #endregion

    #region Item Tracking

    private void TrackingItemUpdate()
    {
        if (trackedItem == null)
        {
            Debug.LogWarning(
                $"{name}: The tracked item is missing.",
                this);

            StartRoaming();
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        bool destinationWasSet =
            SetDestinationNearPosition(trackedItem.position);

        if (!destinationWasSet)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            trackedItem = null;
            StartRoaming();
            return;
        }

        DrawAgentPath(Color.blue);
    }

    #endregion

    #region Movement and Animation

    private bool SetDestinationNearPosition(Vector3 requestedPosition)
    {
        if (!agent.isOnNavMesh)
        {
            return false;
        }

        if (NavMesh.SamplePosition(
                requestedPosition,
                out NavMeshHit hit,
                navMeshSampleDistance,
                agent.areaMask))
        {
            target.value[0] = hit.position;
            return agent.SetDestination(hit.position);
        }

        return false;
    }

    private void PlayAnimation(string animationStateName)
    {
        if (animator == null ||
            string.IsNullOrWhiteSpace(animationStateName))
        {
            return;
        }

        animator.Play(animationStateName);
    }

    private void DrawAgentPath(Color lineColor)
    {
        if (!agent.hasPath || agent.path == null)
        {
            return;
        }

        Vector3[] corners = agent.path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Debug.DrawLine(
                corners[i],
                corners[i + 1],
                lineColor);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (detectionRange != null)
        {
            float roamingRadius =
                Mathf.Abs(detectionRange.localScale.x) * 0.5f;

            Gizmos.DrawWireSphere(
                detectionRange.position,
                roamingRadius);
        }

        if (player != null)
        {
            // Player detection distance.
            Gizmos.DrawWireSphere(
                transform.position,
                playerDetectionDistance);

            // Distance where the monster loses the player.
            Gizmos.DrawWireSphere(
                transform.position,
                losePlayerDistance);

            // Player catching distance.
            Gizmos.DrawWireSphere(
                transform.position,
                catchDistance);
        }
    }
}