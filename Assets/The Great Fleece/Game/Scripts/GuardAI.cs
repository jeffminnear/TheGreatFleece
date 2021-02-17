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
    public NavMode _navMode;
    public List<Transform> waypoints;
    [Tooltip("Index of the first destination. Use 1 if guard is positioned at 0, etc.")]
    public int _startWaypointIndex = 0;
    [Tooltip("Only applies to 'Branch' NavMode. When the guard reaches this waypoint, he will randomly move to one of the remaining waypoints and then reverse from there. Must be lower than the index of the last waypoint.")]
    public int _branchStartWaypointIndex = 0;

    public List<Transform> _w_points;
    private int _currentTarget;
    private NavMeshAgent g_Agent;
    private Animator g_Animator;
    private bool _walk = false;
    private bool _reversing = false;

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
        _w_points = GetLocalWaypoints();
        _currentTarget = _startWaypointIndex;

        g_Agent = gameObject.GetComponent<NavMeshAgent>();
        g_Animator = gameObject.GetComponent<Animator>();

        VerifyComponents(gameObject, g_Agent, g_Animator);

        if (_w_points.Count > 0 && _w_points[_currentTarget] != null)
        {
            g_Agent.SetDestination(_w_points[_currentTarget].position);
        }
        else
        {
            g_Agent.SetDestination(transform.position);
        }
    }

    List<Transform> GetLocalWaypoints()
    {
        if (_navMode == NavMode.Branch)
        {
            if (waypoints.Count == 0 || _branchStartWaypointIndex >= waypoints.Count - 1)
            {
                return waypoints;
            }
            List<Transform> waypointsCopy = new List<Transform>(waypoints);
            List<Transform> startList = waypointsCopy.GetRange(0, _branchStartWaypointIndex + 1);
            waypointsCopy.RemoveRange(0, _branchStartWaypointIndex + 1);
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
            if (_walk)
            {
                SetNextWaypoint();
            }
        }
        else
        {            
            // make sure walking animation is running
            if (!_walk)
            {
                StartCoroutine(TurnAndWalk());
            }
        }
    }

    void SetWalk(bool val)
    {
        _walk = val;
        g_Animator.SetBool("Walk", val);
    }

    void SetNextWaypoint()
    {
        int maxIndex = _w_points.Count - 1;
        if (maxIndex < 0)
        {
            return;
        }

        if (_reversing)
        {
            if (_currentTarget > 0)
            {
                _currentTarget = 0;
                SetWaypoint(_currentTarget);
            }
            else // back to first waypoint
            {
                _w_points = GetLocalWaypoints();
                _reversing = false;
                _currentTarget++;
                StartCoroutine(SetWaypointAfterPause(_currentTarget));
            }
        }
        else
        {
            if (_currentTarget < maxIndex)
            {
                _currentTarget++;
                SetWaypoint(_currentTarget);
            }
            else // last waypoint in list
            {
                switch (_navMode)
                {
                    case NavMode.Loop:
                        _currentTarget = 0;
                        SetWaypoint(_currentTarget);
                        break;
                    case NavMode.Reverse:
                    case NavMode.Branch:
                        _reversing = true;
                        _currentTarget--;
                        StartCoroutine(SetWaypointAfterPause(_currentTarget));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SetWaypoint(int i)
    {
        if (_w_points[i] != null)
        {
            g_Agent.SetDestination(_w_points[i].position);
        }
    }

    float GetPatrolPauseDelay()
    {
        return Random.Range(2f, 5f);
    }

    IEnumerator SetWaypointAfterPause(int waypointIndex)
    {
        SetWalk(false);

        yield return new WaitForSeconds(GetPatrolPauseDelay());

        SetWaypoint(waypointIndex);
    }

    IEnumerator TurnAndWalk()
    {
        yield return new WaitForSeconds(0.25f);

        SetWalk(true);
    }
}
