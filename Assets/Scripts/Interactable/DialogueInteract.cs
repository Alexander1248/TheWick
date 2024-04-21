using NPC;
using UnityEngine;
using Utils;
using Dialogue = Plugins.DialogueSystem.Scripts.DialogueGraph.Dialogue;

namespace Interactable
{
    public class DialogueInteract : MonoBehaviour, IInteractable
    {
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private string dialogueName;
        
        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        private string _buff;
        private bool _dialogueStarted;

        public void Interact(PlayerInteract playerInteract)
        {
            _buff = gameObject.tag;
            gameObject.tag = "Untagged";
            _dialogueStarted = true;
            dialogue.StartDialogue(dialogueName);
            dialogue.onDialogueEnd.AddListener(EndInteract);
        }

        private void EndInteract()
        {
            if (!_dialogueStarted) return;
            gameObject.tag = _buff;
            _dialogueStarted = false;
        }
    }
}