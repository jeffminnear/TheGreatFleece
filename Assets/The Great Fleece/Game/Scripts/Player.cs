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

    private GameObject p_MovePoint;
    private NavMeshAgent p_Agent;
    private Animator p_Animator;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float turnSpeed = 8f;
    private bool isWalking = false;
    private Coroutine turnCoroutine;
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
        p_MovePoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p_MovePoint.GetComponent<MeshRenderer>().enabled = false;
        p_MovePoint.transform.position = transform.position;
        p_MovePoint.SetActive(false);
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
            SetPlayerMovePoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) // Right Click
        {
            ThrowCoinAtPosition(Input.mousePosition);
        }
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
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
            }
            p_MovePoint.transform.position = hit.point;
            p_MovePoint.SetActive(true);
            p_Agent.SetDestination(p_MovePoint.transform.position);
            turnCoroutine = StartCoroutine(TurnTowardsPointAndAct(p_MovePoint.transform.position, PlayerActionWalk));
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
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
            }
            coinTarget = hit.point;
            turnCoroutine = StartCoroutine(TurnTowardsPointAndAct(coinTarget, PlayerActionThrowCoin));
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
            coinsRemaining--;
            Instantiate(coinPrefab, coinTarget, Quaternion.identity);
        }
    }

    IEnumerator TurnTowardsPointAndAct(Vector3 point, PlayerAction action)
    {
        Quaternion lookRotation = Quaternion.LookRotation(point - transform.position);
        Quaternion targetRotation = new Quaternion(transform.rotation.x, lookRotation.y, transform.rotation.z, lookRotation.w);

        while (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) < 0.98f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);

        action();
    }
}
