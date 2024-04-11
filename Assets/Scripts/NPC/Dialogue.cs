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
        [SerializeField] private UDictionary<string, TMP_Text> narrators;
        [SerializeField] private SentenceGraph graph;
        public string locale;

        
        private SentenceData _currentData;
        private TMP_Text _currentNarrator;
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
            if (_wait) return;
            
            _time += Time.deltaTime;
            _currentNarrator.SetText(_currentData.sentence[..(int)(_currentData.sentence.Length * _time / _currentData.time)]);
            
            if (_time > _currentData.time) 
                Invoke(nameof(GoToNext), _currentData.delay);
        }

        private void GoToNext()
        {
            _current = _current.GetNext();
            SwitchUpdate();
        }

        private void SwitchUpdate()
        {
            _currentNarrator.SetText("");
            _current.Update(this);
            _currentData = _current.GetSentence(locale);
            _currentNarrator = narrators.TryGetValue(_currentData.narrator, out var narrator) ? narrator : narrators.Values[0];
            _wait = false;
            _time = 0;
        }
    }
}