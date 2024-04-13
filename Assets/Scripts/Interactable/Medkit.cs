using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    public class Medkit : MonoBehaviour, IInteractable
    {
        [SerializeField] private float medkitUsageTime = 3f;
        [SerializeField] private float medkitHealth = 30f;
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

        private float _healPerCylinder;
        private float _cylinderUsageTime;
        private float _time;
        private float _cylinderTime;
        private bool _selected;
        
        public void Interact(PlayerInteract playerInteract)
        {
            if (audioSource)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
            }

            var childCount = cylinderContainer.childCount;
            _cylinderUsageTime = 0.98f / childCount;
            _healPerCylinder = medkitHealth / childCount;
            _cylinderTime = 0;
            
            if (_time < 0.01f) _time = 0.01f;
        }

        private void Start()
        {
            _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        public void Selected()
        {
            _selected = true;
            _playerHealth.reloadImage.fillAmount = 1 - _time;
        }
        public void Deselected()
        {
            _selected = false;
            _playerHealth.reloadImage.fillAmount = 0;
        }
        
        private void Update()
        {
            if (_time == 0) return;
            if (!Input.GetKey(tipButton) || !_selected)
            {
                return;
            }

            var deltaTime = Time.deltaTime / medkitUsageTime;
            _time += deltaTime;
            _cylinderTime += deltaTime;

            while (cylinderContainer.childCount > 0 && _cylinderTime > _cylinderUsageTime)
            {
                _cylinderTime -= _cylinderUsageTime;
                _playerHealth.EditHealth(_healPerCylinder);
                DestroyImmediate(cylinderContainer.GetChild(0).gameObject);
            }

            _playerHealth.reloadImage.fillAmount = 1 - _time;

            if (_time < 1) return;
            _time = 0;
            gameObject.tag = "Untagged";
            DestroyImmediate(this);
        }
    }
}