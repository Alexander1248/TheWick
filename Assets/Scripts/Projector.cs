using System;
using NPC;
using UnityEngine;
using Utils;

public class Projector : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private UDictionary<string, Sprite> screens;

    private void Start()
    {
        dialogue.StartDialogue();
        dialogue.PauseDialogue();
    }

    public void ChangeScreen(string tag)
    {
        if (screens.TryGetValue(tag, out var screen))
            renderer.sprite = screen;
    }
}