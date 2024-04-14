using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem destructionEffect;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float radiusPlayerHit;
    private Transform player;
    private bool destroyed;

    public void init(Vector3 dir){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.forward = dir;
    }

    void Update(){
        if (destroyed) return;
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, player.position) < radiusPlayerHit){
            hit(player.GetComponent<Health>());
        }
    }

    void hit(Health hp){
        if (destroyed) return;
        if (audioSource != null) audioSource.Play();
        destroyed = true;
        if (hp) {
            hp.DealDamage(damage, (hp.transform.position - transform.position).normalized, 2);
            Debug.Log("Player");
        }
        if (destructionEffect != null){
            destructionEffect.Play();
            destructionEffect.transform.SetParent(null);
            Destroy(destructionEffect.gameObject, 3);
        }
        Destroy(gameObject);
    }

     void OnTriggerEnter(Collider other){
        if (other.CompareTag("Boss")) return;
        Debug.Log(other.tag);

        if (other.CompareTag("Player")){
            hit(other.GetComponent<Health>());
            return;
        }
        hit(null);
     }
}
