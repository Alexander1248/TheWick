using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image reloadImage; // Для вызова через аптечку
    [SerializeField] private float maxHP = 100;
    [SerializeField] private UnityEvent onDeath;
    [FormerlySerializedAs("onDamageDeal")] [SerializeField] private UnityEvent<float, float> onHealthEdit;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float _hp;

    [SerializeField] private int amountRestore;

    [SerializeField] private bool autoHeal;

    [SerializeField] private float autoHealStartTime = 10;
    [SerializeField] private float autoHealStartCooldown = 1;
    [SerializeField] private float autoHealAmount = 3;

    [SerializeField] private MilkShake.ShakePreset preset;

    [SerializeField] private bool IMPLAYER;

    [SerializeField] private ParticleSystem blood;


    private void Start()
    {
        _hp = maxHP;

    }
    
    void Heal()
    {
        EditHealth(autoHealAmount);
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

        if (IMPLAYER && preset != null) MilkShake.Shaker.ShakeAll(preset);

        if (autoHeal){
            CancelInvoke(nameof(Heal));
            InvokeRepeating(nameof(Heal), autoHealStartTime, autoHealStartCooldown);
        }
        EditHealth(-damage);
    }

    public void EditHealth(float delta)
    {
        _hp += delta;
        if (_hp >= maxHP){
            _hp = maxHP;
            CancelInvoke(nameof(Heal));
        }

        if (_hp > 0)
        {
            onHealthEdit.Invoke(_hp, maxHP);
            return;
        }
        onDeath.Invoke();
    }
}