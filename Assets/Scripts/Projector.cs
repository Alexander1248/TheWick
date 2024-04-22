using UnityEngine;
using Utils;
using Dialogue = Plugins.DialogueSystem.Scripts.DialogueGraph.Dialogue;

public class Projector : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private UDictionary<string, Sprite> screens;

    private void Start()
    {
        dialogue.StartDialogueNow("Projector");
        dialogue.PauseDialogue();
    }

    public void ChangeScreen(string tag)
    {
        if (screens.TryGetValue(tag, out var screen))
            renderer.sprite = screen;
    }
}