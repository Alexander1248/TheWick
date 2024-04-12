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

    private RaycastHit _hit;
    void Start(){
        lastScrollTime = Time.time;
    }

    void Update()
    {
        float scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Time.time - lastScrollTime > scrollThreshold)
        {
            int lastweapon = currentWeaponIndex;
            if (scrollWheelInput > 0)
                currentWeaponIndex++;
            else if (scrollWheelInput < 0)
                currentWeaponIndex--;
            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weapons.Length - 1);

            if (scrollWheelInput != 0 && currentWeaponIndex != lastweapon){
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i] != null){
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

            if (!Physics.SphereCast(camera.transform.position, wrenchRayRadius,
                    camera.transform.forward, out _hit, wrenchDistance, enemyMask)) return;
            var health = _hit.collider.gameObject.GetComponent<Health>();
            if (health.IsUnityNull()) return;
            health.DealDamage(wrenchDamage, -transform.forward, wrenchKickForce, _hit.point);
            Debug.Log("Hit! Damage: " + wrenchDamage);
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
}
