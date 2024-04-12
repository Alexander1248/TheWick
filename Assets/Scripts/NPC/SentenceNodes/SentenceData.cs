using System;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SentenceData
    {
        public string sentence;
        public float time = 1;
        public float delay = 1;
        public string narrator = "any";

        public SentenceData() { }

        public SentenceData(string sentence, float time, float delay, string narrator)
        {
            this.sentence = sentence;
            this.time = time;
            this.delay = delay;
            this.narrator = narrator;
        }

        public SentenceData Clone()
        {
            return new SentenceData(sentence, time, delay, narrator);
        }
    }
}