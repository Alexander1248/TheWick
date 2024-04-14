using System;
using System.Collections.Generic;
using System.Linq;
using NPC.SentenceNodes;
using UnityEditor;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "Sentence Graph")]
    public class SentenceGraph : ScriptableObject
    {
        [HideInInspector] public DialogueRoot root;
        [HideInInspector] public List<SelectableDialogueRoot> roots = new();
        [HideInInspector] public List<Sentence> nodes = new();
    }
}