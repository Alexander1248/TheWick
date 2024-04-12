using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoRun : MonoBehaviour
{
    public UnityEvent autorun;
    void Start()
    {
        autorun.Invoke();
    }
}
