using UnityEngine;

namespace NPC
{
    public class DialogueZone : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            if (dialogue.IsStarted) dialogue.PlayDialogue();
            else dialogue.StartDialogue();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            dialogue.PauseDialogue();
        }
    }
}