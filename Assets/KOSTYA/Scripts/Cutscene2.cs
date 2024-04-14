using System.Collections;
using System.Collections.Generic;
using NPC;
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
    
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private AudioSource alarm;
    [SerializeField] private float afterAlarmVolume = 0.5f;
    [SerializeField] private float afterCutsceneVolume = 0.2f;

    private float _volume;
    void Start()
    {
        _volume = alarm.volume;
        playableDirector.Play();
        dialogue.StartDialogue();
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

    void Update()
    {
        alarm.volume = Mathf.Lerp(alarm.volume, _volume, 0.3f);
        
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
        dialogue.StartDialogue("UnderTable");
    }

    public void TryStartAlarm(string tag)
    {
        if (tag != "alarm") return;
        alarm.Play();
    }
    public void TryDecreaseAlarmVolume(string tag)
    {
        if (tag != "alarm") return;
        _volume *= afterAlarmVolume;
    }
    public void TryDecreaseAlarmVolumeAgain(string tag)
    {
        if (tag != "cs end") return;
        _volume *= afterCutsceneVolume;
    }
}
