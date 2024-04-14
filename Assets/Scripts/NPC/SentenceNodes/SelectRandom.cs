using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Utils;
using Random = UnityEngine.Random;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SelectRandom : Sentence
    {
        
        public override Sentence GetNext()
        {
            var index = Random.Range(0, next.Count);
            return next[new List<int>(next.Keys)[index]];
        }

        public override SentenceData GetSentence(string locale)
        {
            return null;
        }

        public override Sentence Clone()
        {
            var sentence = Instantiate(this);
            foreach (var key in next.Keys)
                sentence.next[key] = next[key] == null ? null : next[key].Clone();
            return sentence;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Add Branch", _ => next[next.Count] = null);
            evt.menu.AppendAction("Remove Branch", _ => next.Remove(next.Count - 1));
        }
    }
}