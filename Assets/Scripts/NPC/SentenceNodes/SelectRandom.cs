using System;
using Random = UnityEngine.Random;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SelectRandom : Sentence
    {
        
        public SelectRandom() : base(2) { }
        
        public override Sentence GetNext()
        {
            return next[Random.Range(0, next.Length)];
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