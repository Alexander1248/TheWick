using System;
using NPC;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        dialogue.StartDialogue();
        Destroy(gameObject);
    }
}