using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class ZoneTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent enter;
        [SerializeField] private UnityEvent exit;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            enter.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            exit.Invoke();
        }
    }
}