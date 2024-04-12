using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NPC.SentenceNodes
{
    public abstract class Sentence : ScriptableObject
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 nodePos;
        
        [HideInInspector] public Sentence[] next;

        protected Sentence(int variantCount)
        {
            next = new Sentence[variantCount];
        }

        public abstract Sentence GetNext();
        public abstract SentenceData GetSentence(String locale);

        public virtual void GetState(Dialogue dialogue) { }

        public virtual Sentence Clone()
        {
            var sentence = Instantiate(this);
            sentence.next = new Sentence[next.Length];
            for (var i = 0; i < next.Length; i++) 
                sentence.next[i] = next[i].Clone();
            return sentence;
        }
    }
}