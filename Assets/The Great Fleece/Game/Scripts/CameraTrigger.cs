using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraPosition;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Camera.main.transform.position = _cameraPosition.transform.position;
            Camera.main.transform.rotation = _cameraPosition.transform.rotation;
        }
    }
}
