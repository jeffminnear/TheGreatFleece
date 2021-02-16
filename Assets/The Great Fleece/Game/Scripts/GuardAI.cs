using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public enum NavMode
    {
        Loop,
        Reverse
    }
    public NavMode _navMode;
    public List<Transform> g_Waypoints;
    public int _waypointsStartIndex = 0;

    private int _currentTarget;
    private NavMeshAgent g_Agent;
    [SerializeField]
    private bool g_Walk = false;
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
        _currentTarget = _waypointsStartIndex;

        g_Agent = gameObject.GetComponent<NavMeshAgent>();

        if (g_Waypoints.Count > 0 && g_Waypoints[_currentTarget] != null)
        {
            g_Agent.SetDestination(g_Waypoints[_currentTarget].position);
        }
        else
        {
            g_Agent.SetDestination(transform.position);
        }
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
            if (g_Walk)
            {
                g_Walk = false;
                SetNextWaypoint();
            }
        }
        else
        {            
            // make sure walking animation is running
            if (!g_Walk)
            {
                g_Walk = true;
            }
        }
    }

    void SetNextWaypoint()
    {
        int maxIndex = g_Waypoints.Count - 1;
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
        if (g_Waypoints[i] != null)
        {
            g_Agent.SetDestination(g_Waypoints[i].position);
        }
    }

    float GetPatrolPauseDelay()
    {
        return Random.Range(2f, 5f);
    }

    IEnumerator SetWaypointAfterPause(int waypointIndex)
    {
        yield return new WaitForSeconds(GetPatrolPauseDelay());

        SetWaypoint(waypointIndex);
    }
}
