using System;
using System.Collections.Generic;
using NPC.SentenceNodes;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class LocalizedSentence : Sentence
    {
        public Dictionary<String, SentenceData> sentences = new();

        public LocalizedSentence() : base(1) { }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return sentences.TryGetValue(locale, out var sentence) ? sentence : sentences["en"];
        }

        public override void Update(Dialogue dialogue)
        {
            throw new NotImplementedException();
        }
    }
}