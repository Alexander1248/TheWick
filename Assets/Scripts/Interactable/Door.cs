using UnityEngine;
using Utils;

namespace Interactable
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator; 
        public Transform doorObj;
        private float _t;
        private float _startAngle;
        private float _endAngle;
        private string _state = "CloseDoor";

        [SerializeField] private bool locked = true;

        private Transform _plr;

        [SerializeField] private string lockedMessage = "LOCKED";

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;


        private void Start()
        {
            _plr = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void Interact(PlayerInteract playerInteract)
        {
            if (locked)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                if (!animator.enabled) animator.enabled = true;
                animator.CrossFade("LockedDoor", 0.1f, 0, 0);
                return;
            }
            animator.enabled = false;
            audioSource.clip = clips[1];
            audioSource.Play();

            _state = _state == "CloseDoor" ? "OpenDoor" : "CloseDoor";

            _startAngle = doorObj.localEulerAngles.y;
            var direction = _plr.position - doorObj.position;
            var dotProduct = Vector3.Dot(direction, doorObj.right);
            _endAngle = _state switch
            {
                "OpenDoor" when dotProduct >= 0 => -90,
                "OpenDoor" when dotProduct < 0 => 90,
                _ => 0
            };
            _t = 0.01f;
        }

        private void Update()
        {
            if (_t == 0) return;
            _t += Time.deltaTime;

            doorObj.localEulerAngles = new Vector3(0, Mathf.LerpAngle(_startAngle, _endAngle, _t), 0);

            if (_t >= 1) _t = 0;
        }

    }
}
