using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LurkerHallwayAI : MonoBehaviour
{
    public enum BellStayMode
    {
        Timed,
        UntilAnotherBell
    }

    [Header("References")]
    public NavMeshAgent agent;

    [Header("Patrol")]
    [Tooltip("Assign these in hallway order.")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;

    [Header("Bell Response")]
    public BellStayMode bellStayMode = BellStayMode.Timed;
    public float bellWaitTime = 5f;

    private int currentPatrolIndex = 0;
    private int patrolDirection = 1; // 1 forward, -1 backward

    private bool isInvestigatingBell = false;
    private bool isWaiting = false;

    private Coroutine waitCoroutine;
    private RingTheBell currentBellTarget;

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        RingTheBell.OnBellRung += HandleBellRung;
    }

    private void OnDisable()
    {
        RingTheBell.OnBellRung -= HandleBellRung;
    }

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("LurkerHallwayAI needs a NavMeshAgent.");
            enabled = false;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("LurkerHallwayAI needs at least one patrol point.");
            enabled = false;
            return;
        }

        MoveToPatrolPoint(currentPatrolIndex);
    }

    private void Update()
    {
        if (agent.pathPending)
            return;

        if (HasReachedDestination() && !isWaiting)
        {
            if (isInvestigatingBell)
            {
                if (bellStayMode == BellStayMode.Timed)
                {
                    StartNewWaitCoroutine(WaitAtBellThenResume());
                }
                else if (bellStayMode == BellStayMode.UntilAnotherBell)
                {
                    StayAtBellIndefinitely();
                }
            }
            else
            {
                StartNewWaitCoroutine(WaitAtPatrolThenContinue());
            }
        }
    }

    private void HandleBellRung(RingTheBell bell)
    {
        if (bell == null)
            return;

        currentBellTarget = bell;
        isInvestigatingBell = true;

        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        isWaiting = false;
        agent.isStopped = false;
        agent.SetDestination(bell.transform.position);
    }

    private void StartNewWaitCoroutine(IEnumerator routine)
    {
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);

        waitCoroutine = StartCoroutine(routine);
    }

    private IEnumerator WaitAtPatrolThenContinue()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        AdvancePatrolIndexPingPong();
        MoveToPatrolPoint(currentPatrolIndex);
        isWaiting = false;
        waitCoroutine = null;
    }

    private IEnumerator WaitAtBellThenResume()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(bellWaitTime);

        agent.isStopped = false;
        isInvestigatingBell = false;
        currentBellTarget = null;

        MoveToPatrolPoint(currentPatrolIndex);
        isWaiting = false;
        waitCoroutine = null;
    }

    private void StayAtBellIndefinitely()
    {
        isWaiting = true;
        agent.isStopped = true;
    }

    private void MoveToPatrolPoint(int index)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (index < 0 || index >= patrolPoints.Length)
            return;

        agent.SetDestination(patrolPoints[index].position);
    }

    private void AdvancePatrolIndexPingPong()
    {
        if (patrolPoints.Length <= 1)
            return;

        currentPatrolIndex += patrolDirection;

        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = patrolPoints.Length - 2;
            patrolDirection = -1;
        }
        else if (currentPatrolIndex < 0)
        {
            currentPatrolIndex = 1;
            patrolDirection = 1;
        }
    }

    private bool HasReachedDestination()
    {
        if (!agent.hasPath)
            return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.05f;
    }
}