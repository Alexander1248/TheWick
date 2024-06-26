﻿using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    [Serializable]
    public class Narrator
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private AudioSource source;
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        public void Clear()
        {
            text.SetText("");
        }
        public void Speak(string sentence)
        {
            text.SetText(sentence);
            if (source.IsUnityNull() || sentence.Length % 2 != 0 || source.isPlaying) return;
            source.pitch = Random.Range(minPitch, maxPitch);
            source.Play();
        }
        
    }
}