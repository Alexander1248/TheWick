using System;
using System.Collections;
using System.Collections.Generic;
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

    void Start(){
        lastScrollTime = Time.time;
    }

    void Update()
    {
        float scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Time.time - lastScrollTime > scrollThreshold)
        {
            if (scrollWheelInput > 0)
                currentWeaponIndex++;
            else if (scrollWheelInput < 0)
                currentWeaponIndex--;

            if (scrollWheelInput != 0){
                currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weapons.Length - 1);
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i] != null){
                        weapons[i].SetActive(i == currentWeaponIndex);
                        animatorsWeapons[i].enabled = false;
                        animatorsWeapons[i].Rebind();
                        animatorsWeapons[i].Update(0f);
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
            Invoke("resetGaechnii", gaechniiHit.length);
        }
        else if (Input.GetMouseButton(0) && canShoot && currentWeaponIndex == 2){
            if (!playerAnimator.enabled) playerAnimator.enabled = true;
            playerAnimator.Play("PlayerShoot", 0, 0);
            canShoot = false;

            if (!animatorGun.enabled) animatorGun.enabled = true;
            animatorGun.Play(gunShoot.name, 0, 0);
            Invoke("resetGun", gunShoot.length);
        }
    }

    void resetGaechnii(){
        canHitGaechnii = true;
    }
    void resetGun(){
        canShoot = true;
    }
}
