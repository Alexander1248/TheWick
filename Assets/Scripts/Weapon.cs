using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int compressedSteamCylinderMax = 10;
    [SerializeField] private int compressedSteamCylinder = 10;
    [SerializeField] private BlockyBar compressedSteamCylinderBar;
    
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private MilkShake.ShakePreset shootShaker;
    [Space]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float rayRadius = 0.1f;
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float kickForce;
    private float rollbackTime = 0.2f;
    [SerializeField] private int clipSize = 5;
    [SerializeField] private BlockyBar clipBar;
    
    private float reloadTime = 3.0f;
    [SerializeField] private Animator reload;
    
    [Space]
    [SerializeField] private LayerMask candleMask;
    [SerializeField] private float candleMultiplier = 2;


    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator animatorGun;
    [SerializeField] private AnimationClip gunShoot;
    [SerializeField] private AnimationClip gunReload;


    private RaycastHit _hit;
    private RaycastHit[] _hits;
    private bool _reloading;
    private bool _canShoot = true;
    private int _clipCount;


    public void AddCompressedSteamCylinder(int count)
    {
        compressedSteamCylinder = Math.Min(compressedSteamCylinder + count, compressedSteamCylinderMax);
        if (compressedSteamCylinderBar) compressedSteamCylinderBar.Set(compressedSteamCylinder);
    }

    private void Start()
    {
        rollbackTime = gunShoot.length;
        reloadTime = gunReload.length;

        _hits = new RaycastHit[16];
        reload.speed = 1 / reloadTime;
        if (compressedSteamCylinderBar)
        {
            compressedSteamCylinderBar.Initialize(
                compressedSteamCylinderMax,
                new Vector2(1, 0),
                new Vector2Int(96, 48),
                new Vector2Int(10, 10),
                1);
            compressedSteamCylinderBar.Set(compressedSteamCylinder);
        }

        if (clipBar)
        {
            clipBar.Initialize(
                clipSize,
                new Vector2(0, 0),
                new Vector2Int(48, 48),
                new Vector2Int(10, 10),
                1);
            clipBar.Set(_clipCount);
        }
        Reload();
    }

    private void Update()
    {
        if (!_reloading && Input.GetKeyDown(KeyCode.R))
        {
            if (compressedSteamCylinder <= 0) return;
            compressedSteamCylinder--;
            if (compressedSteamCylinderBar) 
                compressedSteamCylinderBar.Set(compressedSteamCylinder);

            _reloading = true;
            _canShoot = false;
            Invoke(nameof(Reload), reloadTime);
            reload.Play("Reload", -1, 0);
            animatorGun.Play(gunReload.name, 0, 0);
        }
        
        if (_clipCount <= 0) return;
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        if (!_canShoot) return;
        shootParticle.time = 0;
        shootParticle.Play();

        if (!playerAnimator.enabled) playerAnimator.enabled = true;
        playerAnimator.Play("PlayerShoot", 0, 0);
        if (!animatorGun.enabled) animatorGun.enabled = true;
        animatorGun.Play(gunShoot.name, 0, 0);
        
        _clipCount--;
        if (clipBar) clipBar.Set(_clipCount);
        
        _canShoot = false;
        Invoke(nameof(Rollback), rollbackTime);
        
        int count = Physics.SphereCastNonAlloc(transform.position, rayRadius,  transform.forward, _hits, distance, candleMask);
        if (Physics.SphereCast(transform.position,rayRadius, transform.forward, out _hit, distance, enemyMask))
        {
            var health = _hit.collider.gameObject.GetComponent<Health>();
            var dmg = damage;

            for (var index = 0; index < count; index++)
            {
                if (_hits[index].distance >= _hit.distance) break;
                dmg *= candleMultiplier;
                Destroy(_hits[index].collider.gameObject);
            }


            if (health.IsUnityNull()) return;
            health.DealDamage(dmg, -transform.forward, kickForce, _hit.point);
            Debug.Log("Hit! Damage: " + dmg);
        }
        else for (var index = 0; index < count; index++)
            Destroy(_hits[index].collider.gameObject);

        
        MilkShake.Shaker.ShakeAll(shootShaker);
    }

    private void Reload()
    {
        Rollback();
        _reloading = false;
        _clipCount = clipSize;
        if (clipBar) clipBar.Set(_clipCount);
    }
    private void Rollback()
    {
        _canShoot = true;
    }
}