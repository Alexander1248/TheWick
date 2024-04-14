using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Cutscene2 : MonoBehaviour
{
    [SerializeField] private Animator animatorFather;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private FirstPersonController2 firstPersonController2;

    [SerializeField] private PlayableDirector playableDirector;
    private bool catching;
    private bool goingtotable;
    private bool imundertable;

    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform posPlayer;

    [SerializeField] private Transform posTable;

    [SerializeField] private Transform posUnderTable;
    [SerializeField] private PlayableDirector playableDirector2;


    [SerializeField] private Transform posPlayGame;
    [SerializeField] private GameObject[] destroyMeAfterCS;

    [SerializeField] private Animator autoDoor;

    void Start(){
        playableDirector.Play();
    }

    bool isStaying()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    return true;
                }
            }
        }
        return false;
    }

    public void getToPlayer(){
        firstPersonController2.lockPlayer();
        agent.enabled = true;
        agent.SetDestination(firstPersonController2.transform.position);
        animatorFather.CrossFade("FatherWalk", 0.5f, -1, 0);
        catching = true;
    }

    void Update(){
        if (catching && Vector3.Distance(agent.transform.position, firstPersonController2.transform.position) <= 1f){
            agent.transform.forward = (new Vector3(firstPersonController2.transform.position.x, 
                                                agent.transform.position.y,
                                                firstPersonController2.transform.position.z) - agent.transform.position).normalized;
            catching = false;
            animatorFather.CrossFade("FatherTakeUs", 0.5f, -1, 0);
            Invoke("TakeUs", 1.05f);
        }

        if (goingtotable && isStaying()){
            playableDirector2.Play();
            goingtotable = false;
        }
    }

    void TakeUs(){
        playerRb.isKinematic = true;
        firstPersonController2.transform.SetParent(posPlayer);
        firstPersonController2.transform.localPosition = Vector3.zero;
        agent.SetDestination(posTable.position);
        goingtotable = true;
    }

    public void putUnderTable(){
        if (imundertable){
            firstPersonController2.transform.SetParent(null);
            firstPersonController2.transform.position = posPlayGame.position;
            for(int i = 0; i < destroyMeAfterCS.Length; i++) Destroy(destroyMeAfterCS[i]);
            firstPersonController2.unlockPlayer();
            autoDoor.enabled = true;
            autoDoor.Play("OpenAutoDoor", -1, 0);
            playerRb.isKinematic = false;
            return;
        }
        firstPersonController2.transform.SetParent(posUnderTable);
        imundertable = true;
        firstPersonController2.transform.localPosition = Vector3.zero;
    }
}
