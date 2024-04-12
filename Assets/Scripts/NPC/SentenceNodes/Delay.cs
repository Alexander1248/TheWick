using System;
using NPC.SentenceNodes;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class Delay : Sentence
    {
        public float delay;
        
        public Delay() : base(1) { }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return new SentenceData("", -1, delay, "");
        }

        public override Sentence Clone()
        {
            var sentence = Instantiate(this);
            sentence.next = new Sentence[next.Length];
            for (var i = 0; i < next.Length; i++) 
                sentence.next[i] = next[i].Clone();

            sentence.delay = delay;
            return sentence;
        }
    }
}