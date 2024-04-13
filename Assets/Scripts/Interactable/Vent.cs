using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private float valveRotationAngle = 90f;
        [SerializeField] private Animator animator;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;


        private float _time;
        private bool _selected;
        private FirstPersonController2 _controller;

        private void Start()
        {
            _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController2>();
        }

        public void Interact(PlayerInteract playerInteract)
        {
            _controller.inVent = !_controller.inVent;
        }

        public void Selected()
        {
            
        }
        public void Deselected()
        {
            
        }
    }
}