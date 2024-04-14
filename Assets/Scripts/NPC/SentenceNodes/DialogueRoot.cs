using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class DialogueRoot : Sentence
    {
        public DialogueRoot() : base()
        {
            next[0] = null;
        }
        
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
            foreach (var key in next.Keys)
                sentence.next[key] = next[key];
            return sentence;
        }
    }
}