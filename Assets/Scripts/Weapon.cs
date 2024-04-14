using System;
using Interactable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GasInventory inventory;
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

    [SerializeField] private PlayerInteract playerInteract;

    private RaycastHit _hit;
    private RaycastHit[] _hits;
    private bool _reloading;
    private bool _canShoot = true;
    private int _clipCount;

    [SerializeField] private GameObject textNoAmmo;

    [SerializeField] private PauseManager pauseManager;

    private void Start()
    {
        rollbackTime = gunShoot.length;
        reloadTime = gunReload.length;

        _hits = new RaycastHit[16];
        reload.speed = 1 / reloadTime;

        if (clipBar)
        {
            clipBar.Initialize(
                clipSize,
                new Vector2(1, 0),
                new Vector2Int(48, 48),
                new Vector2Int(10, 10),
                1);
            clipBar.Set(_clipCount);
        }
        Reload();
    }

    private void Update()
    {
        if (pauseManager.paused) return;
        if (_clipCount <= 0 && !textNoAmmo.activeSelf) textNoAmmo.SetActive(true);
        else if (_clipCount > 0 && textNoAmmo.activeSelf) textNoAmmo.SetActive(false);

        if (!_reloading && Input.GetKeyDown(KeyCode.R))
        {
            if (inventory.IsEmpty()) return;
            inventory.Edit(-1);

            _reloading = true;
            _canShoot = false;
            Invoke(nameof(Reload), reloadTime);
            reload.enabled = true;
            reload.Play("Reload", -1, 0);
            animatorGun.Play(gunReload.name, 0, 0);
            if (playerInteract) playerInteract.enabled = false;
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

    private void OnEnable()
    {
        if (clipBar) clipBar.gameObject.SetActive(true);
        inventory.EnableBar();
    }

    private void OnDisable()
    {
        if (clipBar) clipBar.gameObject.SetActive(false);
        inventory.DisableBar();
        textNoAmmo.SetActive(false);
    }

    private void Reload()
    {
        reload.enabled = false;
        Rollback();
        _reloading = false;
        _clipCount = clipSize;
        if (clipBar) clipBar.Set(_clipCount);
        if (playerInteract) playerInteract.enabled = true;
    }
    private void Rollback()
    {
        _canShoot = true;
    }
}