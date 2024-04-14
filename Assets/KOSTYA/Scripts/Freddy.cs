using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Freddy : MonoBehaviour
{
    [SerializeField] private FieldOfView fov;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head;
    [SerializeField] private float fightDistance;
    [SerializeField] private AnimationClip[] fightClips;
    [SerializeField] private AnimationClip takeDamageClip;
    [SerializeField] private float legKickForce;
    [SerializeField] private float handKickForce;
    [SerializeField] private float robotDamage = 15;

    [SerializeField] private float seeAfterLostTime;

    [SerializeField] private Collider[] partsColliders;
    [SerializeField] private Rigidbody[] partsRBs;

    private Transform player;
    private Rigidbody rbPlayer;
    private Health hpPlayer;
    private Vector3 normalHead;

    private bool pushing;
    private float pushingforce;
    private float pushingtime;

    [SerializeField] private AudioSource audioSourceWalk;
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioClip[] mainClips;
    [SerializeField] private AudioSource otherSounds;
    [SerializeField] private AudioClip[] otherClips;

    public enum State{
        Idle, Chasing, Fighting, Died, StoppingChase, Stunned
    };
    public State state;

    void Start(){
        normalHead = head.forward;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rbPlayer = player.GetComponent<Rigidbody>();
        hpPlayer = player.GetComponent<Health>();
        state = State.Idle;
    }

    void stopFight(){
        state = State.Idle;
        animator.CrossFade("IdleRobot", 0.1f);
        head.forward = normalHead;
    }

    public void AnimationCall(int id){
        if (state == State.Died) return;
        if (Vector3.Distance(transform.position, player.position) > fightDistance) return;

        if (id == 1) // foot kick
        {
            otherSounds.clip = otherClips[0];
            otherSounds.pitch = Random.Range(0.75f, 1.25f);
            otherSounds.Play();
            pushingtime = 0.25f;
            pushingforce = legKickForce;
            pushing = true;
            hpPlayer.DealDamage(robotDamage, (player.position - transform.position).normalized, 0);
        }
        else if (id == 2) // hand kick
        {
            otherSounds.clip = otherClips[0];
            otherSounds.pitch = Random.Range(0.75f, 1.25f);
            otherSounds.Play();
            pushingtime = 0.25f;
            pushingforce = handKickForce;
            pushing = true;
            hpPlayer.DealDamage(robotDamage, (player.position - transform.position).normalized, 0);
        }
        else if (id == -1){
            audioSourceWalk.pitch = Random.Range(0.75f, 1.25f);
            audioSourceWalk.Play();
        }
    }

    void FixedUpdate(){
        if (!pushing) return;
        Vector3 dir = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        rbPlayer.AddForce(dir.normalized * pushingforce, ForceMode.Acceleration);
        pushingtime -= Time.fixedDeltaTime;
        if (pushingtime <= 0) pushing = false;

    }

    void Update(){
        if (state == State.Died || state == State.Stunned) return;

        if (agent.enabled && (state == State.Chasing || state == State.StoppingChase))
            agent.SetDestination(player.position);
        
        if (Vector3.Distance(transform.position, player.position) <= fightDistance){
            transform.forward = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
            if (state == State.Chasing || state == State.Idle){
                state = State.Fighting;
                int randAnim = Random.Range(0, fightClips.Length);
                animator.CrossFade(fightClips[randAnim].name, 0.15f);
                Invoke("stopFight", fightClips[randAnim].length);
            }
        }

        if (fov.canSeePlayer){
            if (state != State.Chasing && state != State.Fighting){
                CancelInvoke("StopChase");
                state = State.Chasing;
                mainSource.clip = mainClips[1];
                mainSource.Play();
                animator.CrossFade("RunRobot", 0.25f);
            }
        }
        else if (!fov.canSeePlayer && (state == State.Chasing || state == State.Fighting)){
            state = State.StoppingChase;
            Invoke("StopChase", seeAfterLostTime);
        }
    }
    void LateUpdate(){
        if (state == State.Chasing || state == State.StoppingChase || state == State.Fighting)
            head.forward = player.position - transform.position;
    }

    void StopChase(){
        state = State.Idle;
        head.forward = normalHead;
        animator.CrossFade("IdleRobot", 0.2f);
        agent.SetDestination(transform.position);

        mainSource.clip = mainClips[0];
        mainSource.Play();
    }

    public void GetDamaged(){
        //CancelInvoke("Unstun");
        otherSounds.clip = otherClips[2];
        otherSounds.pitch = Random.Range(0.75f, 1.25f);
        otherSounds.Play();
        CancelInvoke();
        animator.CrossFade(takeDamageClip.name, 0.1f, -1, 0);
        state = State.Stunned;
        agent.SetDestination(transform.position);
        Invoke("Unstun", takeDamageClip.length);
    }
    void Unstun(){
        CancelInvoke("StopChase");
        state = State.Chasing;
        animator.CrossFade("RunRobot", 0.25f);
    }

    public void DieBitch(){
        if (state == State.Died) return;
        state = State.Died; 

        mainSource.Stop();
        
        otherSounds.clip = otherClips[1];
        otherSounds.Play();

        GetComponent<Collider>().enabled = false;  
        agent.enabled = false;
        animator.Rebind();
        animator.Update(0f);
        animator.enabled = false;
        CancelInvoke();

        for(int i = 0; i < partsColliders.Length; i++){
            partsColliders[i].transform.SetParent(null);
            partsColliders[i].enabled = true;
            partsRBs[i].isKinematic = false;
            partsRBs[i].AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 1.7f, ForceMode.Impulse);
            Destroy(partsRBs[i].gameObject, 5);
        }
        Destroy(gameObject, 5);
    }
}
