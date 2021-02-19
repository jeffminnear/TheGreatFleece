using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform cameraPosition;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Camera.main.transform.position = cameraPosition.transform.position;
            Camera.main.transform.rotation = cameraPosition.transform.rotation;
        }
    }
}
