using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    [SerializeField] private Rigidbody[] floorRBs;

    public void bossDied(){
        for(int i = 0; i < floorRBs.Length; i++){
            floorRBs[i].isKinematic = false;
            floorRBs[i].AddForce(-Vector3.up * 2f, ForceMode.Impulse);
        }
    }
}
