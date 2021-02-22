using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        mainCamera = GameObject.Find("CM Main").transform;

        Helpers.Validation.VerifyReferences(gameObject, mainCamera);
    }
    
    void Update()
    {
        transform.LookAt(mainCamera.position, Vector3.up);
    }
}
