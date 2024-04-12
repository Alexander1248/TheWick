using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    public class Medkit : MonoBehaviour, IInteractable
    {
        [SerializeField] private float medkitUsageTime = 3f;
        [SerializeField] private Transform cylinderContainer;
        

        private Health _playerHealth;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        private float _cylinderUsageTime;
        private float _time;
        private float _cylinderTime;
        
        public void Interact(PlayerInteract playerInteract)
        {
            if (audioSource)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
            }
            
            
            if (_time < 0.01f) _time = 0.01f;
        }

        private void Start()
        {
            _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }
        
        private void Update()
        {
            if (_time == 0) return;
            if (!Input.GetKey(tipButton)) return;
            var deltaTime = Time.deltaTime / medkitUsageTime;
            _time += deltaTime;
            _cylinderTime += deltaTime;

            if (_cylinderTime > _cylinderUsageTime)
            {
                _cylinderTime = 0;
                DestroyImmediate(transform.GetChild(0));
            }
            

            if (_time < 1) return;
            _time = 0;
            DestroyImmediate(this);
        }
    }
}