using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



void Start(){

    //
    legPositions = new Vector3[legPoints.Length];
    ts = new float[legPoints.Length];
    startingPosLeg = new Vector3[legPoints.Length];
    legmoving = new bool[legPoints.Length];
    for(int i = 0; i < legPoints.Length; i++){
        //legPoints[i].position = targets[i].position
        legPositions[i] = legPoints[i].position;
    }
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

        avgPos /= legPoints.Length;
        //body.position = new Vector3(body.position.x, avgPos.y + bodyOffset.y, body.position.z);

        body.position = Vector3.MoveTowards(body.position, new Vector3(body.position.x, avgPos.y + bodyOffset.y, body.position.z), bodyMovingSpeed * Time.deltaTime);

        Quaternion rotation = Quaternion.FromToRotation(body.up, -CalculateNormal()) * body.rotation;
        Vector3 midRot = Quaternion.RotateTowards(body.rotation, rotation, bodyRotationSpeed * Time.deltaTime).eulerAngles;
        midRot.y = 0;
        body.localEulerAngles = midRot;
    }
}
