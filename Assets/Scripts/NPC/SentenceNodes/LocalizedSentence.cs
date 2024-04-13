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

        public LocalizedSentence()
        {
            next[0] = null;
        }
        
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
            foreach (var key in next.Keys)
                sentence.next[key] = next[key].Clone();
            sentences.Keys.ForEach(key => sentence.sentences[key] = sentences[key]);
            return sentence;
        }
    }
}