using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private GameObject _playerMovePoint;
    private NavMeshAgent _agent;

    [SerializeField]
    private float _moveSpeed = 5f;

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

        Move();
    }

    void InitializeMovePoint()
    {
        _playerMovePoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _playerMovePoint.GetComponent<MeshRenderer>().enabled = false;
        _playerMovePoint.SetActive(false);
    }

    void InitializePlayer()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
    }

    void Move()
    {

    }

    void SetPlayerMovePoint(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        int layerMask = 1 << 8;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            _playerMovePoint.transform.position = hit.point;
            _playerMovePoint.SetActive(true);
            _agent.SetDestination(_playerMovePoint.transform.position);
        }
    }
}
