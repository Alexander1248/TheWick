using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SimpleSentence : Sentence
    {
        public SentenceData sentenceData;

        public SimpleSentence()
        {
            next[0] = null;
        }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return sentenceData;
        }

        public override Sentence Clone()
        {
            var sentence = Instantiate(this);
            foreach (var key in next.Keys)
                sentence.next[key] = next[key] == null ? null : next[key].Clone();

            sentence.sentenceData = sentenceData.Clone();
            return sentence;
        }
    }
}