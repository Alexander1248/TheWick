using UnityEngine;
using Utils;

namespace Interactable
{
    public class GasCylinder : MonoBehaviour, IInteractable
    {
        private GasInventory _inventory;
        [SerializeField] private int gasCount = 1;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        
        public void Interact(PlayerInteract playerInteract)
        {
            if (audioSource)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
            }
            _inventory.Edit(gasCount);
            Destroy(gameObject);
        }

        private void Start()
        {
            _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<GasInventory>();
        }
    }
}