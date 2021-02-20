using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static Helpers.Validation;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private GameObject p_MovePoint;
    private NavMeshAgent p_Agent;
    private Animator p_Animator;
    [SerializeField]
    private float moveSpeed = 5f;
    private float turnSpeed = 8f;
    private bool isWalking = false;
    private Coroutine turnCoroutine;

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

    void SetPlayerMovePoint(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        int layerMask = 1 << 8;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            SetWalking(false);
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
            }
            p_MovePoint.transform.position = hit.point;
            p_MovePoint.SetActive(true);
            p_Agent.SetDestination(p_MovePoint.transform.position);
            turnCoroutine = StartCoroutine(TurnTowardsMovePointAndWalk());
        }
    }

    IEnumerator TurnTowardsMovePointAndWalk()
    {
        Quaternion targetRotation = Quaternion.LookRotation(p_MovePoint.transform.position - transform.position);
        targetRotation.x = transform.rotation.x;
        targetRotation.z = transform.rotation.z;

        while (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) < 0.98f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);

        SetWalking(true);
    }
}
