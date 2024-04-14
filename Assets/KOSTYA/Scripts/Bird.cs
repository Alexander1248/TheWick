using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private Animator birdanimator;
    [SerializeField] private GameObject birdObj;
    [SerializeField] private AnimationClip[] animClips;

    public void EnableBird(){
        birdObj.SetActive(true);
        birdanimator.enabled = true;
        birdanimator.Play(animClips[0].name, -1, 0);
        Invoke("startTalking", animClips[0].length);
    }
    public void DisableBird(){
        birdanimator.CrossFade(animClips[2].name, 0.2f, -1, 0);
        Invoke("turnBirdOff", animClips[2].length);
    }

    void turnBirdOff(){
        birdObj.SetActive(false);
    }

    void startTalking(){
        birdObj.SetActive(true);
        birdanimator.CrossFade(animClips[1].name, 0.2f, -1, 0);
    }
}
