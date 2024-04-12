using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class DialogueRoot : Sentence
    {
        public DialogueRoot() : base(1) { }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return null;
        }

        public override Sentence Clone()
        {
            var sentence = Instantiate(this);
            sentence.next = new Sentence[next.Length];
            for (var i = 0; i < next.Length; i++) 
                sentence.next[i] = next[i].Clone();
            return sentence;
        }
    }
}