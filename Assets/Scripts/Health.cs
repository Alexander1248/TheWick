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

    [SerializeField] private MilkShake.ShakePreset preset;

    [SerializeField] private bool IMPLAYER;

    [SerializeField] private ParticleSystem blood;


    private void Start()
    {
        _hp = maxHP;

    }
    
    void Heal(){
        _hp += 3;
        if (_hp >= maxHP){
            _hp = maxHP;
            CancelInvoke(nameof(Heal));
        }
        onDamageDeal.Invoke(_hp, maxHP);
    }

    private Vector3 _buff;
    public void DealDamage(float damage, Vector3 direction, float kickForce , Vector3? point = null)
    {
        if (rb) rb.AddForce(-direction * kickForce, ForceMode.Impulse);

        if (blood){
            blood.transform.position = _buff;
            if (point != null)
            {
                _buff = blood.transform.position;
                blood.transform.position = point.Value;
            }
            if (!IMPLAYER){
                blood.transform.LookAt(direction);
            }
            else blood.transform.forward = Camera.main.transform.forward;
            blood.Play();
        }

        if (IMPLAYER) MilkShake.Shaker.ShakeAll(preset);

        _hp -= damage;
        onDamageDeal.Invoke(_hp, maxHP);
        if (autoHeal){
            CancelInvoke(nameof(Heal));
            InvokeRepeating(nameof(Heal), 10, 1);
        }
        if (_hp <= 0)
        {
            onDeath.Invoke();
            return;
        }
    }
    
    
}