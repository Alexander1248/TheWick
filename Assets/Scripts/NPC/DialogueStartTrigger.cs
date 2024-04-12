using UnityEngine;

namespace NPC
{
    public class DialogueStartTrigger : MonoBehaviour
    {
        public Dialogue dialogue;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            dialogue.StartDialogue();
            Destroy(gameObject);
        }
    }
}