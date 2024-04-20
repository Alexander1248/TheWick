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

            if (hands == null) hands = playerInteract.GetComponent<Hands>();

            if (!hands.requestWrench(wrenchPos)) {
                time = 0;
                return;
            }
            audioSourceWrench.Play();

            if (audioSource)
            {
                audioSource.clip = clips[1];
                audioSource.Play();
            }

            if (time >= 0.01f) return;
            time = 0.001f;

            _startAngle = valveObj.localEulerAngles.y;
            
            if (open) _endAngle = _startAngle - valveRotationAngle;
            else _endAngle = _startAngle + valveRotationAngle;
        }

        public void Selected()
        {
            _selected = true;
        }
        public void Deselected()
        {
            if (time != 0) hands.releaseWrench();
            audioSourceWrench.Stop();
            time = 0;
            _selected = false;
        }

        public void LockValve()
        {
            locked = true;
        }

        private void Update()
        {
            valveProgress.Invoke(open ? Mathf.Clamp01(1 - time) : Mathf.Clamp01(time));
            if (time == 0) return;
            if (Input.GetKeyUp(tipButton) && _selected && hands != null){
                hands.releaseWrench();
                audioSourceWrench.Stop();
                time = 0;
            }
            if (!Input.GetKey(tipButton) || !_selected) return;
            time += Time.deltaTime / valveRotationTime;

            valveObj.localEulerAngles = new Vector3(0, Mathf.LerpAngle(_startAngle, _endAngle, time), 0);

            if (time < 1) return;
            time = 0;
            hands.releaseWrench();
            audioSourceWrench.Stop();
            if (open) valveClosed.Invoke();
            else valveOpened.Invoke();
            open = !open;
        }
    }
}