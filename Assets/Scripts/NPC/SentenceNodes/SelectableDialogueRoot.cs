using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using UnityEngine;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SelectableDialogueRoot : Sentence
    {
        [SerializeField] private string rootName;

        public string RootName => rootName;
        
        public SelectableDialogueRoot()
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