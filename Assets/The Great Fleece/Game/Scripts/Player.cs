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
    private float p_MoveSpeed = 5f;
    private bool p_Walk = false;

    void Awake()
    {
        InitializeMovePoint();
        InitializePlayer();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            SetPlayerMovePoint(Input.mousePosition);
        }

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

        p_Agent.speed = p_MoveSpeed;
        p_Animator.SetBool("Walk", false);
    }

    void HandleMovement()
    {
        if (!IsPlayerAtDestination())
        {
            if (!p_Walk)
            {
                p_Walk = true;
                p_Animator.SetBool("Walk", p_Walk);
            }
        }
        else
        {
            if (p_Walk)
            {
                p_Walk = false;
                p_Animator.SetBool("Walk", p_Walk);
            }
        }
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
            p_MovePoint.transform.position = hit.point;
            p_MovePoint.SetActive(true);
            p_Agent.SetDestination(p_MovePoint.transform.position);
        }
    }
}
