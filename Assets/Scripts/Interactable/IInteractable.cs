using UnityEngine;
using Utils;

namespace Interactable
{
    public interface IInteractable
    {
        void Interact(PlayerInteract playerInteract);
        KeyCode TipButton { get; }
        UDictionary<string, string> TipName { get; }

        MeshRenderer[] MeshesOutline { get; }
        void Selected() {
            
        }
        void Deselected() {
            
        }
    }
}