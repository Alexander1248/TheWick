using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPC.SentenceNodes
{
    public abstract class Sentence : ScriptableObject
    {
        public string guid;
        public Vector2 nodePos;
        
        public Sentence[] next;

        protected Sentence(int variantCount)
        {
            next = new Sentence[variantCount];
        }

        public abstract Sentence GetNext();
        public abstract SentenceData GetSentence(String locale);

        public abstract void Update(Dialogue dialogue);
    }
}