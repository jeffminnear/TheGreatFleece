using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = Camera.main.transform.position;
        transform.position = lastCameraPosition;
    }

    void Update()
    {
        if (Camera.main.transform.position != lastCameraPosition)
        {
            lastCameraPosition = Camera.main.transform.position;
            transform.position = lastCameraPosition;
        }
    }
}
