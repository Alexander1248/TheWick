using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Utils;

namespace NPC.SentenceNodes
{
    public abstract class Sentence : ScriptableObject
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 nodePos;
        
        [HideInInspector] public UDictionary<int, Sentence> next = new();

        public string tag;

        public abstract Sentence GetNext();
        public abstract SentenceData GetSentence(String locale);

        public virtual void GetState(Dialogue dialogue) { }

        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            
        }

        public virtual Sentence Clone()
        {
            var sentence = Instantiate(this);
            foreach (var key in next.Keys)
                sentence.next[key] = next[key].Clone();
            return sentence;
        }
    }
}