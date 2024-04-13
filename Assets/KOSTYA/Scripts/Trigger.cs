using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent valveOpened;
    [SerializeField] private bool destroyAfterCall = true;

    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            valveOpened.Invoke();
            if (destroyAfterCall) Destroy(gameObject);
        }
    }
}
