using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactable
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private List<string> tagsInteract;
        [SerializeField] private List<string> tagsLook;
        [SerializeField] private float distInteract;

        [SerializeField] private Material outlineMat;

        private int _idxInteract = -1;
        private IInteractable _interactable;
        private GameObject _interactableObj;
        private bool _needToHideDescription;

        [SerializeField] private GameObject UITip;
        [SerializeField] private TMP_Text textTipButton;
        [SerializeField] private TMP_Text textTipTip;

        [SerializeField] private TMP_Text textLook;

        void Start()
        {
            InvokeRepeating(nameof(Raycasting), 0.1f, 0.1f);
        }

        private void Update()
        {
            if (_idxInteract != -1 && Input.GetKeyDown(_interactable.TipButton)) InteractObj();
        }

        private RaycastHit _hit;
        private void Raycasting()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, Mathf.Infinity, layerMask))
                CheckObject(_hit.transform.gameObject);
            else
            {
                _idxInteract = -1;
                _interactableObj = null;
                HideTip();
                if (_needToHideDescription) HideDescription();
            }
        }

        private void InteractObj()
        {
            if (Vector3.Distance(transform.position, _interactableObj.transform.position) > distInteract) return;
            _interactable.Interact(this);
        }

        private void CheckObject(GameObject obj)
        {
            _idxInteract = tagsInteract.IndexOf(obj.tag);
            if (_idxInteract == -1)
            {
                _interactableObj = null;
                HideTip();
                return;
            }
            if (_interactableObj != obj && Vector3.Distance(transform.position, obj.transform.position) <= distInteract)
            {
                HideTip();

                _interactableObj = obj;
                _interactable = obj.GetComponent<IInteractable>();
            
                ShowTip(_interactable.TipButton.ToString(), _interactable.TipName.TryGetValue(PlayerPrefs.GetString("Language"), 
                    out var value) ? value : _interactable.TipName.Values[0]);
            }
            else if (Vector3.Distance(transform.position, obj.transform.position) > distInteract)
            {
                HideTip();
                _idxInteract = -1;
                _interactableObj = null;
                _interactable = null;
            }

            //if (idxInteract == 0) obj.GetComponent<door>().Interact();
        }


        public void ShowSubtitle(string obj)
        {
            _needToHideDescription = false;
            ShowDescription(obj);
            CancelInvoke(nameof(HideDescription));
            Invoke(nameof(HideDescription), 2);
        }

        private void ShowDescription(string description)
        {
            textLook.text = description;
        }

        public void HideDescription()
        {
            _needToHideDescription = false;
            textLook.text = "";
        }

        private void ShowTip(string button, string tip)
        {
            UITip.SetActive(true);
            textTipButton.text = button;
            textTipTip.text = tip;

            var meshes = _interactable.MeshesOutline;
            foreach (var mesh in meshes)
            {
                var materials = mesh.materials;
                var newArray = new Material[materials.Length + 1];
                for (int j = 0; j < materials.Length; j++) newArray[j] = materials[j];
                newArray[newArray.Length - 1] = outlineMat;

                mesh.materials = newArray;
            }
        }

        private void HideTip()
        {
            if (!UITip.activeSelf) return;
            UITip.SetActive(false);
            if (_interactable == null) return;
            var meshes = _interactable.MeshesOutline;
            foreach (var mesh in meshes)
            {
                var materials = mesh.materials;
                var newArray = new Material[materials.Length - 1];
                for (int j = 0; j < materials.Length - 1; j++) newArray[j] = materials[j];
                mesh.materials = newArray;
            }
        }
    }
}
