using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static Helpers.Validation;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject playerMovePointPrefab;

    private GameObject p_MovePoint;
    private NavMeshAgent p_Agent;
    private Animator p_Animator;
    private Animator pmp_Animator;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float turnSpeed = 8f;
    [SerializeField]
    private float coinThrowDelay = 1f;
    private bool isWalking = false;
    private Coroutine turnCoroutine;
    private bool finishedTurning = true;
    private Coroutine throwCoroutine;
    private bool finishedThrowing = true;
    private Vector3 coinTarget;
    [SerializeField]
    private int coinsRemaining = 1;

    void Awake()
    {
        InitializeMovePoint();
        InitializePlayer();
    }

    void Update()
    {
        GetInput();
        HandleMovement();
    }

    void InitializeMovePoint()
    {
        p_MovePoint = Instantiate(playerMovePointPrefab, transform.position, Quaternion.identity);
        pmp_Animator = p_MovePoint.GetComponent<Animator>();

        VerifyComponents(p_MovePoint, pmp_Animator);
    }

    void InitializePlayer()
    {
        p_Agent = gameObject.GetComponent<NavMeshAgent>();
        p_Animator = gameObject.GetComponentInChildren<Animator>();

        VerifyComponents(gameObject, p_Agent, p_Animator);

        p_Agent.speed = moveSpeed;
        p_Animator.SetBool("Walk", false);
    }

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            CancelCoroutine(throwCoroutine, finishedThrowing, SetFinishedThrowing, "CancelThrow");
            SetPlayerMovePoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) // Right Click
        {
            CancelCoroutine(throwCoroutine, finishedThrowing, SetFinishedThrowing, "CancelThrow");
            ThrowCoinAtPosition(Input.mousePosition);
        }
    }

    void CancelCoroutine(Coroutine coroutine, bool finished, SetFinished setFinished)
    {
        if (coroutine != null && !finished)
        {
            StopCoroutine(coroutine);
            setFinished(true);
        }
    }

    void CancelCoroutine(Coroutine coroutine, bool finished, SetFinished setFinished, string animationTrigger)
    {
        if (coroutine != null && !finished)
        {
            StopCoroutine(coroutine);
            setFinished(true);
            p_Animator.SetTrigger(animationTrigger);
        }
    }

    delegate void SetFinished(bool finished);

    void SetFinishedThrowing(bool val)
    {
        finishedThrowing = val;
    }

    void SetFinishedTurning(bool val)
    {
        finishedTurning = val;
    }

    void HandleMovement()
    {
        if (IsPlayerAtDestination())
        {
            if (isWalking)
            {
                SetWalking(false);
            }
        }
    }

    void SetWalking(bool val)
    {
        p_Agent.isStopped = !val;
        isWalking = val;
        p_Animator.SetBool("Walk", val);
    }

    bool IsPlayerAtDestination()
    {
        return (Vector3.Distance(transform.position, p_MovePoint.transform.position) < p_Agent.stoppingDistance);
    }

    bool IsFloorHit(Vector3 position, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        int layerMask = 1 << 8;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetPlayerMovePoint(Vector3 position)
    {
        RaycastHit hit;
        if (IsFloorHit(position, out hit))
        {
            SetWalking(false);
            CancelCoroutine(turnCoroutine, finishedTurning, SetFinishedTurning);
            p_MovePoint.transform.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
            pmp_Animator.SetTrigger("Sparkle");
            p_Agent.SetDestination(p_MovePoint.transform.position);
            turnCoroutine = StartCoroutine(TurnTowardsPointAndAct(p_MovePoint.transform.position, SetFinishedTurning, PlayerActionWalk));
        }
    }

    void ThrowCoinAtPosition(Vector3 position)
    {
        if (coinsRemaining <= 0)
        {
            return;
        }
        
        RaycastHit hit;
        if (IsFloorHit(position, out hit))
        {
            SetWalking(false);
            CancelCoroutine(turnCoroutine, finishedTurning, SetFinishedTurning);
            coinTarget = hit.point;
            turnCoroutine = StartCoroutine(TurnTowardsPointAndAct(coinTarget, SetFinishedTurning, PlayerActionThrowCoin));
        }
    }

    delegate void PlayerAction();

    void PlayerActionWalk()
    {
        SetWalking(true);
    }

    void PlayerActionThrowCoin()
    {
        if (coinTarget != null)
        {
            throwCoroutine = StartCoroutine(ThrowCoinRoutine(SetFinishedThrowing));
        }
    }

    IEnumerator ThrowCoinRoutine(SetFinished finishedThrowing)
    {
        finishedThrowing(false);

        p_Animator.SetTrigger("Throw");

        yield return new WaitForSeconds(coinThrowDelay);

        coinsRemaining--;
        Instantiate(coinPrefab, coinTarget, Quaternion.identity);

        GuardAI[] guards = GameObject.FindObjectsOfType<GuardAI>();
        foreach( GuardAI guard in guards)
        {
            guard.CoinDroppedAtPoint(coinTarget);
        }

        finishedThrowing(true);
    }

    IEnumerator TurnTowardsPointAndAct(Vector3 point, SetFinished finishedTurning, PlayerAction action)
    {
        finishedTurning(false);

        Quaternion lookRotation = Quaternion.LookRotation(point - transform.position);
        Quaternion targetRotation = new Quaternion(transform.rotation.x, lookRotation.y, transform.rotation.z, lookRotation.w);

        while (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) < 0.98f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);
        finishedTurning(true);
        action();
    }
}
