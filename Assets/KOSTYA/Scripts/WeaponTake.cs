using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public class WeaponTake : MonoBehaviour, IInteractable
{
        [SerializeField] private KeyCode tipButton;
        [SerializeField] private UDictionary<string, string> tipName;
        [SerializeField] private MeshRenderer[] meshesOutline;
        
        [SerializeField] private AudioSource audioSource;
        public KeyCode TipButton => tipButton;
        public UDictionary<string, string> TipName => tipName;
        public MeshRenderer[] MeshesOutline => meshesOutline;


        [SerializeField] private int idWeapon;
        [SerializeField] private UnityEvent valveOpened;

        
        public void Interact(PlayerInteract playerInteract)
        {
            if (audioSource)
            {
                audioSource.Play();
            }
            GameObject.FindGameObjectWithTag("Player").GetComponent<Hands>().UnlockWeapon(idWeapon);
            valveOpened.Invoke();
            Destroy(gameObject);
        }
}
