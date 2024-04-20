using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;

namespace Interactable
{
    [ExecuteInEditMode]
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

        [SerializeField] private AudioSource audioSourceWrench;

        [SerializeField][Range(0, 1)] private float time;
        private bool _selected;
        private bool _wrench;

        [SerializeField] private Transform wrenchPos;
        private Hands hands;
        
        // TODO: Fix valve 
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

            if (hands.IsUnityNull()) hands = playerInteract.GetComponent<Hands>();

            if (!hands.requestWrench(wrenchPos))
            {
                _wrench = false;
                return;
            }
            _wrench = true;
            audioSourceWrench.Play();

            if (audioSource)
            {
                audioSource.clip = clips[1];
                audioSource.Play();
            }
            
            _startAngle = valveObj.localEulerAngles.y;
            if (time >= 0.01f)
            {
                var t = Mathf.Clamp01(time);
                if (open)
                {
                    _startAngle += valveRotationAngle * t; 
                    _endAngle = _startAngle - valveRotationAngle;
                }
                else
                {
                    _startAngle -= valveRotationAngle * t; 
                    _endAngle = _startAngle + valveRotationAngle;
                }

                return;
            }

            time = 0.001f;
            if (open) _endAngle = _startAngle - valveRotationAngle;
            else _endAngle = _startAngle + valveRotationAngle;
        }

        public void Selected()
        {
            _selected = true;
        }
        public void Deselected()
        {
            if (_wrench)
            {
                hands.releaseWrench();
                audioSourceWrench.Stop();
            }
            _wrench = false;
            _selected = false;
        }

        public void LockValve()
        {
            locked = true;
        }

        private void Update()
        {
            if (time == 0) return;
            valveProgress.Invoke(open ? Mathf.Clamp01(1 - time) : Mathf.Clamp01(time));
            if (Input.GetKeyUp(tipButton) && _selected && _wrench && hands != null){
                hands.releaseWrench();
                audioSourceWrench.Stop();
                _wrench = false;
            }
            if (!Input.GetKey(tipButton) || !_selected || !_wrench) return;
            time += Time.deltaTime / valveRotationTime;

            var angles = valveObj.localEulerAngles;
            angles = new Vector3(angles.x, Mathf.Lerp(_startAngle, _endAngle, time), angles.z);
            valveObj.localEulerAngles = angles;

            if (time < 1) return;
            time = 0;
            hands.releaseWrench();
            audioSourceWrench.Stop();
            _wrench = false;
            if (open) valveClosed.Invoke();
            else valveOpened.Invoke();
            open = !open;
        }
    }
}