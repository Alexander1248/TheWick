using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hands : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Animator animatorGaechnii;
    [SerializeField] private AnimationClip gaechniiHit;
    [SerializeField] private Animator animatorGun;
    [SerializeField] private AnimationClip gunShoot;
    private bool canHitGaechnii = true;
    private bool canShoot = true;


    private int currentWeaponIndex = 0;
    [SerializeField] private float scrollThreshold = 0.2f;
    private float lastScrollTime = 0f;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Animator[] animatorsWeapons;
    [Space]
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float wrenchRayRadius = 0.5f;
    [SerializeField] private float wrenchDistance = 3.0f;
    [SerializeField] private float wrenchDamage = 10.0f;
    [SerializeField] private float wrenchKickForce = 1f;

    [SerializeField] private bool wrenchUnlocked;
    [SerializeField] private bool weaponUnlocked;

    [SerializeField] private Transform wrenchForVent;
    private bool venting;

    private RaycastHit _hit;
    void Start(){
        lastScrollTime = Time.time;
    }

    public void UnlockWeapon(int id){ // 1 - wrench, 2 - gun
        if (id == 1){
            wrenchUnlocked = true;
            currentWeaponIndex = 1;
        }
        else if (id == 2){
            weaponUnlocked = true;
            currentWeaponIndex = 2;
        }
        TakeWeapon();
    }

    void TakeWeapon(){
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null) continue;
            weapons[i].SetActive(i == currentWeaponIndex);
            animatorsWeapons[i].enabled = true;
            animatorsWeapons[i].Rebind();
            animatorsWeapons[i].Update(0f);
            if (currentWeaponIndex == 2){
                animatorsWeapons[i].Play("TakeWeapon", 0, 0);
            }
            else if (currentWeaponIndex == 1){
                animatorsWeapons[i].Play("TakeGaechnii", 0, 0);
            }
        }
    }

    public bool requestWrench(Transform pos){
        if (weapons[1].activeSelf){
            wrenchForVent.gameObject.SetActive(true);
            wrenchForVent.SetParent(pos);
            wrenchForVent.localEulerAngles = Vector3.zero;
            wrenchForVent.localPosition = Vector3.zero;
            wrenchForVent.localScale = Vector3.one;
            venting = true;
            weapons[1].SetActive(false);
            return true;
        }
        return false;
    }
    
    public void releaseWrench(){
        Debug.Log("sdf");
        wrenchForVent.gameObject.SetActive(false);
        venting = false;
        if (wrenchUnlocked)
        {
            currentWeaponIndex = 1;
            TakeWeapon();
        }
    }

    public void wrenchHit(){
        if (!Physics.SphereCast(camera.transform.position - camera.transform.forward, wrenchRayRadius,
                camera.transform.forward, out _hit, wrenchDistance, enemyMask)) return;
        var health = _hit.collider.gameObject.GetComponent<Health>();
        if (health == null) return;
        health.DealDamage(wrenchDamage, -transform.forward, wrenchKickForce, _hit.point);
    }

    void Update()
    {
        if (venting) return;

        float scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Time.time - lastScrollTime > scrollThreshold)
        {
            int lastweapon = currentWeaponIndex;
            if (scrollWheelInput > 0)
                currentWeaponIndex++;
            else if (scrollWheelInput < 0)
                currentWeaponIndex--;
            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weapons.Length - 1);

            if (currentWeaponIndex == 1 && !wrenchUnlocked) currentWeaponIndex = lastweapon;
            if (currentWeaponIndex == 2 && !weaponUnlocked) currentWeaponIndex = lastweapon;

            if (scrollWheelInput != 0 && currentWeaponIndex != lastweapon){
                TakeWeapon();
                lastScrollTime = Time.time;
            }
        }

        if (Input.GetMouseButton(0) && canHitGaechnii && currentWeaponIndex == 1){
            if (!playerAnimator.enabled) playerAnimator.enabled = true;
            playerAnimator.Play("PlayerGaechniiHit", 0, 0);
            canHitGaechnii = false;

            if (!animatorGaechnii.enabled) animatorGaechnii.enabled = true;
            animatorGaechnii.Play(gaechniiHit.name, 0, 0);
            Invoke(nameof(ResetGaechnii), gaechniiHit.length);


        }
        //else if (Input.GetMouseButton(0) && canShoot && currentWeaponIndex == 2){
        //    if (!playerAnimator.enabled) playerAnimator.enabled = true;
        //    playerAnimator.Play("PlayerShoot", 0, 0);
        //    canShoot = false;

        //    if (!animatorGun.enabled) animatorGun.enabled = true;
        //    animatorGun.Play(gunShoot.name, 0, 0);
        //    Invoke("resetGun", gunShoot.length);
        //}
    }

    private void ResetGaechnii(){
        canHitGaechnii = true;
    }

    private void ResetGun(){
        canShoot = true;
    }

    public void OnEnable()
    {
        if (weapons[currentWeaponIndex] == null) return;
        weapons[currentWeaponIndex].SetActive(true);
    }
    public void OnDisable()
    {
        foreach (var weapon in weapons)
            if (weapon != null) 
                weapon.SetActive(false);
        
    }
}
