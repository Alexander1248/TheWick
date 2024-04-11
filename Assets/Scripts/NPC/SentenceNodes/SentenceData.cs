using System;

namespace NPC.SentenceNodes
{
    [Serializable]
    public class SentenceData
    {
        public String sentence;
        public float time = 1;
        public float delay = 1;
        public string narrator = "any";
    }
}