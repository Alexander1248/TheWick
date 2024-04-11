using System;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int compressedSteamCylinderMax = 10;
    [SerializeField] private int compressedSteamCylinder = 10;
    [SerializeField] private BlockyBar compressedSteamCylinderBar;
    
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private Animator animator;
    [SerializeField] private MilkShake.ShakePreset shootShaker;
    [Space]
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float kickForce;
    [SerializeField] private float rollbackTime = 0.2f;
    
    [SerializeField] private int clipSize = 5;
    [SerializeField] private float reloadTime = 3.0f;
    [SerializeField] private Animator reload;

    private RaycastHit _hit;
    private bool _reloading = false;
    private bool _canShoot = true;
    private int _clipCount;


    public void AddCompressedSteamCylinder(int count)
    {
        compressedSteamCylinder = Math.Min(compressedSteamCylinder + count, compressedSteamCylinderMax);
        compressedSteamCylinderBar.Set(compressedSteamCylinder);
    }

    private void Start()
    {
        reload.speed = 1 / reloadTime;
        Reload();
        compressedSteamCylinderBar.Initialize(compressedSteamCylinderMax, new Vector2(1, 0), new Vector2Int(96, 48), new Vector2Int(10, 10), 1);
    }

    private void Update()
    {
        if (!_reloading && Input.GetKeyDown(KeyCode.R))
        {
            if (compressedSteamCylinder <= 0) return;
            compressedSteamCylinder--;
            compressedSteamCylinderBar.Set(compressedSteamCylinder);

            _reloading = true;
            _canShoot = false;
            Invoke(nameof(Reload), reloadTime);
            reload.Play("Reload", -1, 0);
        }
        
        if (_clipCount <= 0) return;
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        shootParticle.time = 0;
        shootParticle.Play();
        
        _clipCount--;
        _canShoot = false;
        Invoke(nameof(Rollback), rollbackTime);
        MilkShake.Shaker.ShakeAll(shootShaker);
        
        Physics.Raycast(transform.position, transform.forward, out _hit, distance, mask);
        if (_hit.collider.IsUnityNull()) return;
        var health = _hit.collider.gameObject.GetComponent<Health>();
        if (health.IsUnityNull()) return;
        health.DealDamage(damage, transform.forward * kickForce, _hit.point);
    }

    private void Reload()
    {
        Rollback();
        _reloading = false;
        _clipCount = clipSize;
    }
    private void Rollback()
    {
        _canShoot = true;
    }
}