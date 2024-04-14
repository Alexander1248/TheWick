using System;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class AutoRun : MonoBehaviour
    {
        [SerializeField] private UnityEvent autorun;
        private void Start()
        {
            autorun.Invoke();
            Destroy(gameObject);
        }
    }
}