using System;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float damage;
    [SerializeField] private float kickForce;

    private RaycastHit _hit;

    private void Update()
    {
        if (!Input.GetButtonDown(PlayerPrefs.GetString("Attack"))) return;
        
        Physics.Raycast(transform.position, transform.forward, out _hit, distance, mask);
        if (_hit.IsUnityNull()) return;
        var health = _hit.collider.gameObject.GetComponent<Health>();
        if (health.IsUnityNull()) return;
        health.DealDamage(damage, transform.forward * kickForce);
    }
}