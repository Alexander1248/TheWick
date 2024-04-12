using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem destructionEffect;

    public void init(Vector3 dir){
        transform.forward = dir;
    }

    void Update(){
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

     void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            other.GetComponent<Health>().DealDamage(damage, (other.transform.position - transform.position).normalized, 2);
        }
        if (destructionEffect != null){
            destructionEffect.Play();
            destructionEffect.transform.SetParent(null);
            Destroy(destructionEffect.gameObject, 3);
        }
        Destroy(gameObject);
     }
}
