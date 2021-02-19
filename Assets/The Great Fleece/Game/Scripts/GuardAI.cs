using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static Helpers.Validation;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GuardAI : MonoBehaviour
{
    public enum NavMode
    {
        Loop,
        Reverse,
        Branch
    }
    public NavMode navMode;
    public List<Transform> waypoints;
    [Tooltip("Index of the first destination. Use 1 if guard is positioned at 0, etc.")]
    public int _startWaypointIndex = 0;
    [Tooltip("Only applies to 'Branch' NavMode. When the guard reaches this waypoint, he will randomly move to one of the remaining waypoints and then reverse from there. Must be lower than the index of the last waypoint.")]
    public int branchStartWaypointIndex = 0;

    private List<Transform> localWaypoints;
    private int currentTarget;
    private NavMeshAgent g_Agent;
    private Animator g_Animator;
    private bool isWalking = false;
    private bool isReversing = false;

    void Awake()
    {
        InitializeGuard();
    }

    void Update()
    {
        HandleMovement();
    }

    void InitializeGuard()
    {
        localWaypoints = GetLocalWaypoints();
        currentTarget = _startWaypointIndex;

        g_Agent = gameObject.GetComponent<NavMeshAgent>();
        g_Animator = gameObject.GetComponent<Animator>();

        VerifyComponents(gameObject, g_Agent, g_Animator);

        if (localWaypoints.Count > 0 && localWaypoints[currentTarget] != null)
        {
            g_Agent.SetDestination(localWaypoints[currentTarget].position);
        }
        else
        {
            g_Agent.SetDestination(transform.position);
        }
    }

    List<Transform> GetLocalWaypoints()
    {
        if (navMode == NavMode.Branch)
        {
            if (waypoints.Count == 0 || branchStartWaypointIndex >= waypoints.Count - 1)
            {
                return waypoints;
            }
            List<Transform> waypointsCopy = new List<Transform>(waypoints);
            List<Transform> startList = waypointsCopy.GetRange(0, branchStartWaypointIndex + 1);
            waypointsCopy.RemoveRange(0, branchStartWaypointIndex + 1);
            startList.Add(waypointsCopy[Random.Range(0, waypointsCopy.Count)]);
            return startList;
        }

        return waypoints;
    }

    bool IsGuardAtDestination()
    {
        return (Vector3.Distance(transform.position, g_Agent.destination) < g_Agent.stoppingDistance);
    }

    void HandleMovement()
    {
        if (IsGuardAtDestination())
        {
            // make sure walking animation has stopped
            if (isWalking)
            {
                SetNextWaypoint();
            }
        }
        else
        {            
            // make sure walking animation is running
            if (!isWalking)
            {
                StartCoroutine(TurnAndWalk());
            }
        }
    }

    void SetWalking(bool val)
    {
        isWalking = val;
        g_Animator.SetBool("Walk", val);
    }

    void SetNextWaypoint()
    {
        int maxIndex = localWaypoints.Count - 1;
        if (maxIndex < 0)
        {
            return;
        }

        if (isReversing)
        {
            if (currentTarget > 0)
            {
                currentTarget = 0;
                SetWaypoint(currentTarget);
            }
            else // back to first waypoint
            {
                localWaypoints = GetLocalWaypoints();
                isReversing = false;
                currentTarget++;
                StartCoroutine(SetWaypointAfterPause(currentTarget));
            }
        }
        else
        {
            if (currentTarget < maxIndex)
            {
                currentTarget++;
                SetWaypoint(currentTarget);
            }
            else // last waypoint in list
            {
                switch (navMode)
                {
                    case NavMode.Loop:
                        currentTarget = 0;
                        SetWaypoint(currentTarget);
                        break;
                    case NavMode.Reverse:
                    case NavMode.Branch:
                        isReversing = true;
                        currentTarget--;
                        StartCoroutine(SetWaypointAfterPause(currentTarget));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SetWaypoint(int i)
    {
        if (localWaypoints[i] != null)
        {
            g_Agent.SetDestination(localWaypoints[i].position);
        }
    }

    float GetPatrolPauseDelay()
    {
        return Random.Range(2f, 5f);
    }

    IEnumerator SetWaypointAfterPause(int waypointIndex)
    {
        SetWalking(false);

        yield return new WaitForSeconds(GetPatrolPauseDelay());

        SetWaypoint(waypointIndex);
    }

    IEnumerator TurnAndWalk()
    {
        yield return new WaitForSeconds(0.25f);

        SetWalking(true);
    }
}
