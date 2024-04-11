using System;
using NPC.SentenceNodes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace NPC
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private UDictionary<string, Narrator> narrators;
        [SerializeField] private SentenceGraph graph;
        public string locale;

        
        private SentenceData _currentData;
        private Narrator _currentNarrator;
        private Sentence _current;
        private double _time;
        private bool _wait = true;


        public void StartDialogue()
        {
            _current = graph.root;
            SwitchUpdate();
        }

        public void Update()
        {
            if (_current.IsUnityNull()) return;
            if (_currentData == null) GoToNext();
            if (_wait) return;
            
            _time += Time.deltaTime;

            if (_time >= _currentData.time)
            {
                Invoke(nameof(GoToNext), Math.Max(0, _currentData.delay));
                _wait = true;
                return;
            }

            _currentNarrator.Speak(_currentData.sentence[..(int)((_currentData.sentence.Length + 1) * _time / _currentData.time)]);
        }

        private void GoToNext()
        {
            _currentNarrator?.Clear();
            _current = _current.GetNext();
            SwitchUpdate();
        }

        private void SwitchUpdate()
        {
            if (_current.IsUnityNull()) return;
            _current.GetState(this);
            _currentData = _current.GetSentence(locale);
            _wait = false;
            _time = 0;
            if (_currentData.IsUnityNull()) return;
            _currentNarrator = narrators.TryGetValue(_currentData.narrator, out var narrator) ? narrator : narrators.Values[0];
        }
    }
}