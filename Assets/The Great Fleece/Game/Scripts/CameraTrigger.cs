using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform cameraPosition;
    private Transform mainCamera;

    void Start()
    {
        mainCamera = GameObject.Find("CM Main").transform;

        Helpers.Validation.VerifyReferences(gameObject, mainCamera);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            mainCamera.position = cameraPosition.transform.position;
            mainCamera.rotation = cameraPosition.transform.rotation;
        }
    }
}
