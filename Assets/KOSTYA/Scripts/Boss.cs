using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    private Vector3[] legPositions;

    [SerializeField] private Transform[] legPoints;
    [SerializeField] private Transform[] targets;
    [SerializeField] private Vector3 legOffset;
    private bool[] legmoving;
    private float[] ts;
    private Vector3[] startingPosLeg;
    private Vector3 avgPos = Vector3.zero;
    [SerializeField] private Transform body;

    [SerializeField] private float stepDist;
    [SerializeField] private float legMoveSpeed;
    [SerializeField] private AnimationCurve stepHeightBehave;
    [SerializeField] private float stepHeight;
    [SerializeField] private Vector3 bodyOffset;
    [SerializeField] private float bodyRotationSpeed;
    [SerializeField] private float bodyMovingSpeed;


    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float walkRadius;

    private Transform player;

    [SerializeField] private GameObject bigBullet;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private float gunSpeedFollow;
    [SerializeField] private AnimationClip shootGunClip;
    [SerializeField] private ParticleSystem shootParticles;
    [SerializeField] private Transform bulletSpawPos;


    [SerializeField] private Transform gun;
    [SerializeField] private Transform gunHandler;
    [SerializeField] private Vector3[] maxminAnglesGun;

    [SerializeField] private AnimationClip clipStayShoot;
    [SerializeField] private Transform[] spawnPointsTurret;
    [SerializeField] private GameObject bulletTurret;
    [SerializeField] private Transform turret1;
    [SerializeField] private Vector3[] maxminAnglesTurret;
    [SerializeField] private ParticleSystem shootParticlesTurret;
    private int kright;


    [SerializeField] private Transform topTurretBase;
    [SerializeField] private Transform topTurretGun;
    [SerializeField] private Transform topTurretSpawnPoint;
    [SerializeField] private ParticleSystem topParticles;
    [SerializeField] private AnimationClip clipTopShoot;
     private int ktop;

    private Rigidbody rbPlayer;
     [SerializeField] private AnimationClip nearHitClip;
     [SerializeField] private float nearRadius;
     [SerializeField] private float pushingforce;
    private bool pushing;
    private float pushingtime;

    public enum State{
        Walking, ShootingBig, ShootingStay, NearAttack
    }
    [SerializeField] private State state;   
        
    void findNewTraget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
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


    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rbPlayer = player.GetComponent<Rigidbody>();
        state = State.Walking;

        //
        legPositions = new Vector3[legPoints.Length];
        ts = new float[legPoints.Length];
        startingPosLeg = new Vector3[legPoints.Length];
        legmoving = new bool[legPoints.Length];
        for(int i = 0; i < legPoints.Length; i++){
            //legPoints[i].position = targets[i].position
            legPositions[i] = legPoints[i].position;
        }

        Invoke("Attack", Random.Range(2f, 5f));
    }

    bool canMoveLeg(int k){
        // custom logic here

        int checking = k == 0 ? 1 : 0;
        if (legmoving[checking]) return false;

        return true;
    }

    Vector3 CalculateNormal()
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 current = targets[i].position;
            Vector3 next = targets[(i + 1) % targets.Length].position;
            sum += Vector3.Cross(current, next);
        }
        return sum.normalized;
    }

    void Update()
    {
        if (state == State.ShootingStay)
        {
            agent.SetDestination(transform.position);
            return;
        }
        if (isStaying()) findNewTraget();
    }

    void FixedUpdate(){
        if (!pushing) return;
        Vector3 dir = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        rbPlayer.AddForce(dir.normalized * pushingforce, ForceMode.Acceleration);
        pushingtime -= Time.fixedDeltaTime;
        if (pushingtime <= 0) pushing = false;

    }

    void LateUpdate(){
        avgPos = Vector3.zero;
        for(int i = 0; i < legPoints.Length; i++){
            RaycastHit hit;
            if (Physics.Raycast(targets[i].position + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity))
            {
                targets[i].position = hit.point;
            }

            if (Vector3.Distance(legPoints[i].position, targets[i].position) >= stepDist && canMoveLeg(i))
            {
                legmoving[i] = true;
                startingPosLeg[i] = legPoints[i].position;
            }

            if (legmoving[i]){
                ts[i] += legMoveSpeed * Time.deltaTime;
                legPositions[i] = Vector3.Lerp(startingPosLeg[i], targets[i].position, ts[i]);
                legPositions[i].y = targets[i].position.y + stepHeightBehave.Evaluate(ts[i]) * stepHeight;
                
                if (legPositions[i] == targets[i].position)
                {
                    legmoving[i] = false;
                    ts[i] = 0;
                }
            }
            legPoints[i].position = legPositions[i] + legOffset;

            avgPos += legPoints[i].position;
        }

        if (Vector3.Distance(transform.position, player.position) <= nearRadius && state == State.Walking){
            state = State.NearAttack;
            CancelInvoke("Attack");
            gunAnimator.enabled = true;
            gunAnimator.CrossFade(nearHitClip.name, 0.2f, -1, 0);
            Invoke("stopAnim", nearHitClip.length);
            Invoke("Attack", nearHitClip.length + Random.Range(2f, 5f));
            Invoke("pushPlayer", 40/60f);
        }

        avgPos /= legPoints.Length;
        //body.position = new Vector3(body.position.x, avgPos.y + bodyOffset.y, body.position.z);

        body.position = Vector3.MoveTowards(body.position, new Vector3(body.position.x, avgPos.y + bodyOffset.y, body.position.z), bodyMovingSpeed * Time.deltaTime);

        Vector3 dir = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);
        Vector3 midRot = Quaternion.RotateTowards(transform.rotation, rotation, bodyRotationSpeed * Time.deltaTime).eulerAngles;
        transform.eulerAngles = midRot;

        rotateGun(gun, false);
        gunHandler.localEulerAngles = -gun.localEulerAngles;

        rotateGun(turret1, true);

        Quaternion rotationgun1 = Quaternion.LookRotation((player.position - topTurretBase.position).normalized);
        topTurretBase.rotation = Quaternion.RotateTowards(topTurretBase.rotation, rotationgun1, gunSpeedFollow * Time.deltaTime);
        topTurretBase.localEulerAngles = new Vector3(0, topTurretBase.localEulerAngles.y, 0);
        topTurretGun.rotation = Quaternion.RotateTowards(topTurretGun.rotation, rotationgun1, gunSpeedFollow * Time.deltaTime);
        topTurretGun.localEulerAngles = new Vector3(topTurretGun.localEulerAngles.x, 0, 0);
    }

    void rotateGun(Transform obj, bool right){
        if (state == State.ShootingBig) return;
        Quaternion rotationgun1 = Quaternion.LookRotation((player.position - obj.position).normalized);

        if (right) rotationgun1 *= Quaternion.Euler(0, -90, 0);

        obj.rotation = Quaternion.RotateTowards(obj.rotation, rotationgun1, gunSpeedFollow * Time.deltaTime);
        if (obj.localEulerAngles.x <= 180 && obj.localEulerAngles.x > maxminAnglesGun[0].x) obj.localEulerAngles = new Vector3(maxminAnglesGun[0].x, obj.localEulerAngles.y, obj.localEulerAngles.z);
        else if (obj.localEulerAngles.x > 180 && obj.localEulerAngles.x < maxminAnglesGun[1].x) obj.localEulerAngles = new Vector3(maxminAnglesGun[1].x, obj.localEulerAngles.y, obj.localEulerAngles.z);
        if (obj.localEulerAngles.y <= 180 && obj.localEulerAngles.y > maxminAnglesGun[0].y) obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, maxminAnglesGun[0].y, obj.localEulerAngles.z);
        else if (obj.localEulerAngles.y > 180 && obj.localEulerAngles.y < maxminAnglesGun[1].y) obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, maxminAnglesGun[1].y, obj.localEulerAngles.z);
        if (obj.localEulerAngles.z <= 180 && obj.localEulerAngles.z > maxminAnglesGun[0].z) obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, obj.localEulerAngles.y, maxminAnglesGun[0].z);
        else if (obj.localEulerAngles.z > 180 && obj.localEulerAngles.z < maxminAnglesGun[1].z) obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, obj.localEulerAngles.y, maxminAnglesGun[1].z);
        obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, obj.localEulerAngles.y, obj.localEulerAngles.z);


    }

    void shootRightTurret(){
        kright --;
        if (kright < 0){
            CancelInvoke("shootRightTurret");
            return;
        }

        shootParticlesTurret.Clear();
        shootParticlesTurret.Play();
        for(int i = 0; i < spawnPointsTurret.Length; i++){
            GameObject obj0 = Instantiate(bulletTurret, spawnPointsTurret[i].position, Quaternion.Euler(0, 0, 0));
            obj0.GetComponent<BulletBoss>().init(spawnPointsTurret[i].right);
        }
    }

    void shootTopTurret(){
        ktop --;
        if (ktop < 0){
            CancelInvoke("shootTopTurret");
            return;
        }

        topParticles.Clear();
        topParticles.Play();
        gunAnimator.CrossFade(clipTopShoot.name, 0.05f, -1, 0);
        GameObject obj0 = Instantiate(bulletTurret, topTurretSpawnPoint.position, Quaternion.Euler(0, 0, 0));
        obj0.GetComponent<BulletBoss>().init(topTurretSpawnPoint.forward);
    }

    void pushPlayer(){
        pushingtime = 0.25f;
        pushing = true;
    }

    void Attack(){
        Invoke("Attack", ktop * 1/3f + Random.Range(2f, 5f));
        state = State.Walking;
        ktop = 10;
        InvokeRepeating("shootTopTurret", 0, 1/3f);
        Invoke("stopAnim", ktop * 1/3f);
        gunAnimator.enabled = true;

        return;
        Invoke("Attack", clipStayShoot.length + Random.Range(2f, 5f));
        state = State.ShootingStay;
        gunAnimator.enabled = true;
        CancelInvoke("stopAnim");
        Invoke("stopAnim", clipStayShoot.length);
        gunAnimator.CrossFade(clipStayShoot.name, 0.2f, -1, 0);
        kright = 10;
        InvokeRepeating("shootRightTurret", 0, 1/3f);

        return;
        Invoke("Attack", Random.Range(2f, 5f));
        state = State.ShootingBig;
        gunAnimator.enabled = true;
        CancelInvoke("stopAnim");
        Invoke("stopAnim", shootGunClip.length);
        gunAnimator.CrossFade(shootGunClip.name, 0.2f, -1, 0);
        shootParticles.Play();
        GameObject obj = Instantiate(bigBullet, bulletSpawPos.position, Quaternion.Euler(0, 0, 0));
        obj.GetComponent<BulletBoss>().init(bulletSpawPos.forward);
    }

    void stopAnim(){
        gunAnimator.Rebind();
        gunAnimator.Update(0f);
        state = State.Walking;
        gunAnimator.enabled = false;
    }
}
