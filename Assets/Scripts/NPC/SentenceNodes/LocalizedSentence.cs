using System;
using System.Collections.Generic;
using NPC.SentenceNodes;
using Utils;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class LocalizedSentence : Sentence
    {
        public UDictionary<String, SentenceData> sentences = new();

        public LocalizedSentence() : base(1) { }
        
        public override Sentence GetNext()
        {
            return next[0];
        }

        public override SentenceData GetSentence(string locale)
        {
            return sentences.TryGetValue(locale, out var sentence) ? sentence : sentences.Values[0];
        }

        public override Sentence Clone()
        {
            var sentence = Instantiate(this);
            sentence.next = new Sentence[next.Length];
            for (var i = 0; i < next.Length; i++) 
                sentence.next[i] = next[i].Clone();
            sentences.Keys.ForEach(key => sentence.sentences[key] = sentences[key]);
            return sentence;
        }
    }
}