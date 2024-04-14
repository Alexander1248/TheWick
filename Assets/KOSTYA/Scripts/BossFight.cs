using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    [SerializeField] private Rigidbody[] floorRBs;
    [SerializeField] private Rigidbody boosRB;
    [SerializeField] protected ParticleSystem[] boomParticles;

        [SerializeField] private Rigidbody[] otherRBs;
        [SerializeField] private GameObject[] candles;
        [SerializeField] private GameObject bottomCollider;

    public void bossDied(){
        for(int i = 0; i < boomParticles.Length; i++) boomParticles[i].Play();
        for(int i = 0; i < floorRBs.Length; i++){
            floorRBs[i].isKinematic = false;
            for(int k = 0; k < floorRBs[i].transform.childCount / 2; k++){
                Destroy(floorRBs[i].transform.GetChild(Random.Range(0, floorRBs[i].transform.childCount)).gameObject);
            }
            floorRBs[i].AddForce(-Vector3.up * 2f, ForceMode.Impulse);
        }
        for(int i = 0; i < otherRBs.Length; i++){
            otherRBs[i].isKinematic = false;
        }
        for(int i = 0; i < candles.Length; i++)
            candles[i].SetActive(false);
        bottomCollider.SetActive(false);
        boosRB.isKinematic = false;
    }
}
