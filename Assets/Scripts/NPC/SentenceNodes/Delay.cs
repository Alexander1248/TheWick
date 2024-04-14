using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class Delay : Sentence
    {
        public float delay;

        public Delay()
        {
            next[0] = null;
        }
        
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
            foreach (var key in next.Keys)
                sentence.next[key] = next[key] == null ? null : next[key].Clone();

            sentence.delay = delay;
            return sentence;
        }
    }
}