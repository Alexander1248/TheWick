using System;
using System.Collections.Generic;
using System.Linq;
using NPC.SentenceNodes;
using Unity.VisualScripting;
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

        public static Sentence Clone(Sentence node)
        {
            var clones = new Dictionary<Sentence, Sentence>();
            var completed = new List<Sentence>();
            var queue = new Queue<Sentence>();
            
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var sentence = queue.Dequeue();
                if (completed.Contains(sentence)) continue;
                completed.Add(sentence);
                clones[sentence] = sentence.Clone();
                foreach (var key in sentence.next.Keys.Where(key => !sentence.next[key].IsUnityNull()))
                    queue.Enqueue(sentence.next[key]);
                
            }
            
            completed.Clear();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var sentence = queue.Dequeue();
                if (completed.Contains(sentence)) continue;
                completed.Add(sentence);

                var clone = clones[sentence];
                foreach (var key in sentence.next.Keys)
                {
                    clone.next[key] = sentence.next[key].IsUnityNull() ? null : clones[sentence.next[key]];
                    if (sentence.next[key].IsUnityNull()) continue;
                    queue.Enqueue(sentence.next[key]);
                }
            }

            return clones[node];
        }
    }
}