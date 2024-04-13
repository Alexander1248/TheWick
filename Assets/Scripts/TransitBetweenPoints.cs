using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitBetweenPoints : StateMachineBehaviour
{
    public bool enter;
    public Vector3 from;
    public Vector3 to;
    public Collider collider;

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var t = stateInfo.normalizedTime;
        if (enter)
        {
            animator.transform.position = new Vector3(
                Mathf.Lerp(from.x, to.x, Mathf.Max(0, 2f * t - 1)),
                Mathf.Lerp(from.y, to.y, Mathf.Max(0, 2f * t)),
                Mathf.Lerp(from.z, to.z, Mathf.Max(0, 2f * t - 1))
            );
        }
        else
        {
            
            animator.transform.position = new Vector3(
                Mathf.Lerp(from.x, to.x, Mathf.Max(0, 2f * t)),
                Mathf.Lerp(from.y, to.y, Mathf.Max(0, 2f * t - 1)),
                Mathf.Lerp(from.z, to.z, Mathf.Max(0, 2f * t))
            );
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        collider.isTrigger = false;
    }
}
