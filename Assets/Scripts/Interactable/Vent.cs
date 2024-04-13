using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        private bool _closed = true;
        private float _time;
        private bool _selected;
        private FirstPersonController2 _controller;

        private void Start()
        {
            _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController2>();
        }

        public void Interact(PlayerInteract playerInteract)
        {
            if (_closed)
            {
                _closed = !_closed;
                animator.Play("OpenVent");
                InvokeRepeating(nameof(ChangeView), 0.1f, 0.1f);
                return; 
            }
            _controller.ChangeVentState();
        }

        public void Selected()
        {
            
        }
        public void Deselected()
        {
            
        }

        public void ChangeView()
        {
            if (animator.GetCurrentAnimatorStateInfo(-1).length >
                animator.GetCurrentAnimatorStateInfo(-1).normalizedTime) return;
            CancelInvoke(nameof(ChangeView));
            _controller.ChangeVentState();
        }
    }
}