using UnityEngine;
using Utils;

namespace Interactable
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float time;
        public Transform inside;
        public Transform outside;
        
        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        private bool _closed = true;
        private bool _animation = true;
        private FirstPersonController2 _controller;


        private void Start()
        {
            _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController2>();
            GetComponent<Collider>();
        }

        public void Interact(PlayerInteract playerInteract)
        {
            if (_closed)
            {
                _closed = false;
                _animation = true;
                animator.speed = 1 / time;
                animator.Play("OpenVent");
                Invoke(nameof(ChangeVentState), time);
                return;
            }

            if (_animation) return;
            _controller.ChangeVentState(this);
        }

        public void Selected()
        {
            
        }
        public void Deselected()
        {
            
        }

        public void ChangeVentState()
        {
            _controller.ChangeVentState(this);
            _animation = false;
        }
    }
}