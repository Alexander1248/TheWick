using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Interactable
{
    public class Valve : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _valveOpenTime;
        [SerializeField] private Animator animator; 
        public Transform valveObj;
        private float _startAngle;
        private float _endAngle;
        
        [SerializeField] private bool open = true;
        
        [SerializeField] private bool locked = true;
        [SerializeField] private UnityAction valveOpened;
        [SerializeField] private UnityAction valveClosed;

        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;


        private float _time;
        
        public void Interact(PlayerInteract playerInteract)
        {
            if (locked)
            {
                
            }
            _startAngle = valveObj.localEulerAngles.y;
            
            if (open) _endAngle = _startAngle - 180 * (1 - _time);
            else _endAngle = _startAngle + 180 * (1 - _time);
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
            _time += Time.deltaTime / _valveOpenTime;

            valveObj.localEulerAngles = new Vector3(0, Mathf.LerpAngle(_startAngle, _endAngle, _time), 0);

            if (_time < 1) return;
            _time = 0;
            if (open) valveClosed.Invoke();
            else valveOpened.Invoke();
            open = !open;
        }
    }
}