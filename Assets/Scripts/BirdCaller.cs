using System.Collections.Generic;
using NPC;
using Plugins.DialogueSystem.Scripts.DialogueGraph.Nodes;
using Plugins.DialogueSystem.Scripts.DialogueGraph.Nodes.PropertyNodes;
using UnityEngine;
using Dialogue = Plugins.DialogueSystem.Scripts.DialogueGraph.Dialogue;

public class BirdCaller : Property
{
    
    [SerializeField] private Stage stage;
    [SerializeField] private bool show;
    private Bird _bird;
    
    
    public override AbstractNode Clone()
    {
        var clone = Instantiate(this);
        clone.stage = stage;
        clone.show = show;
        return clone;
    }

    private void ShowBird(Dialogue dialogue)
    {
        if ((bool)dialogue.buffer.GetValueOrDefault("birdActive", false)) return;
        _bird.EnableBird();
        dialogue.buffer["birdActive"] = true;
    }

    private void HideBird(Dialogue dialogue)
    {
        if (!(bool)dialogue.buffer.GetValueOrDefault("birdActive", false)) return;
        _bird.DisableBird();
        dialogue.buffer["birdActive"] = false;
    }

    public override void OnDrawStart(Dialogue dialogue, Storyline node)
    {
        if (stage != Stage.OnDrawStart) return;
        _bird = GameObject.FindGameObjectWithTag("Player").GetComponent<Bird>();
        if (show) ShowBird(dialogue);
        else HideBird(dialogue);
    }

    public override void OnDrawEnd(Dialogue dialogue, Storyline storyline)
    {
        if (stage != Stage.OnDrawEnd) return;
        _bird = GameObject.FindGameObjectWithTag("Player").GetComponent<Bird>();
        if (show) ShowBird(dialogue);
        else HideBird(dialogue);
    }

    public override void OnDelayStart(Dialogue dialogue, Storyline storyline)
    {
        if (stage != Stage.OnDelayStart) return;
        _bird = GameObject.FindGameObjectWithTag("Player").GetComponent<Bird>();
        if (show) ShowBird(dialogue);
        else HideBird(dialogue);
    }

    public override void OnDelayEnd(Dialogue dialogue, Storyline storyline)
    {
        if (stage != Stage.OnDelayEnd) return;
        _bird = GameObject.FindGameObjectWithTag("Player").GetComponent<Bird>();
        if (show) ShowBird(dialogue);
        else HideBird(dialogue);
    }
}