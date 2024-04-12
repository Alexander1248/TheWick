using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "Sentence Graph")]
    public class SentenceGraph : ScriptableObject
    {
        [HideInInspector] public DialogueRoot root;
        [HideInInspector] public List<Sentence> nodes = new();


        public Sentence CreateNode(Type type)
        {
            var node = CreateInstance(type) as Sentence;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);
            
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            
            return node;
        }

        public void DeleteNode(Sentence node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public static void AddLink(Sentence from, int index, Sentence to)
        {
            from.next[index] = to;
        }
        public void RemoveLink(Sentence from, Sentence to)
        {
            for (var i = 0; i < from.next.Length; i++)
                if (from.next[i] == to)
                    from.next[i] = null;
        }
    }
}