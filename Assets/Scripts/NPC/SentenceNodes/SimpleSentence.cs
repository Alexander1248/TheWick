using System;
using NPC.SentenceNodes;
using UnityEngine.Serialization;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SimpleSentence : Sentence
    {
        public SentenceData sentenceData;
        
        public SimpleSentence() : base(1) { }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return sentenceData;
        }

        public override void Update(Dialogue dialogue)
        {
            throw new NotImplementedException();
        }
    }
}