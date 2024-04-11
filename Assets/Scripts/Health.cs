using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHP = 100;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent<float, float> onDamageDeal;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float _hp;

    [SerializeField] private int amountRestore;

    [SerializeField] private bool autoHeal;

    // [SerializeField] private MilkShake.ShakePreset preset;

    [SerializeField] private bool IMPLAYER;

    [SerializeField] private ParticleSystem blood;


    private void Start()
    {
        _hp = maxHP;

    }
    
    void heal(){
        _hp += 3;
        if (_hp >= maxHP){
            _hp = maxHP;
            CancelInvoke("heal");
        }
        onDamageDeal.Invoke(_hp, maxHP);
    }

    public void DealDamage(float damage, Vector3 kick)
    {
        if (rb) rb.AddForce(kick, ForceMode.Impulse);

        if (blood){
            if (!IMPLAYER){
                blood.transform.LookAt(-kick);
            }
            else blood.transform.forward = Camera.main.transform.forward;
            blood.Play();
        }

        // if (IMPLAYER) MilkShake.Shaker.ShakeAll(preset);

        _hp -= damage;
        onDamageDeal.Invoke(_hp, maxHP);
        if (autoHeal){
            CancelInvoke("heal");
            InvokeRepeating("heal", 10, 1);
        }
        if (_hp <= 0)
        {
            onDeath.Invoke();
            return;
        }
    }
    
    
}