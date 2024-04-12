﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    public class Valve : MonoBehaviour, IInteractable
    {
        [SerializeField] private float valveRotationTime = 3f;
        [SerializeField] private float valveRotationAngle = 90f;
        [SerializeField] private Animator animator; 
        public Transform valveObj;
        private float _startAngle;
        private float _endAngle;
        
        [SerializeField] private bool open = true;
        
        [SerializeField] private bool locked = true;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        [Space]
        [SerializeField] private UnityEvent valveOpened;
        [SerializeField] private UnityEvent valveClosed;
        [SerializeField] private UnityEvent<float> valveProgress;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;


        private float _time;
        
        public void Interact(PlayerInteract playerInteract)
        {
            if (locked)
            {
                if (audioSource)
                {
                    audioSource.clip = clips[0];
                    audioSource.Play();
                }

                if (!animator) return;
                if (!animator.enabled) animator.enabled = true;
                animator.CrossFade("LockedValve", 0.1f, 0, 0);
                return;
            }
            if (animator) animator.enabled = false;
            if (audioSource)
            {
                audioSource.clip = clips[1];
                audioSource.Play();
            }

            _startAngle = valveObj.localEulerAngles.y;
            
            if (open) _endAngle = _startAngle - valveRotationAngle * (1 - _time);
            else _endAngle = _startAngle + valveRotationAngle * (1 - _time);
            _time = 0.01f;
        }

        public void LockValve()
        {
            locked = true;
        }

        private void Update()
        {
            if (_time == 0) return;
            if (!Input.GetKey(tipButton)) return;
            _time += Time.deltaTime / valveRotationTime;

            if (open) valveProgress.Invoke(Mathf.Clamp01(_time));
            else valveProgress.Invoke(Mathf.Clamp01(1 - _time));
            valveObj.localEulerAngles = new Vector3(0, Mathf.LerpAngle(_startAngle, _endAngle, _time), 0);

            if (_time < 1) return;
            _time = 0;
            if (open) valveClosed.Invoke();
            else valveOpened.Invoke();
            open = !open;
        }
    }
}