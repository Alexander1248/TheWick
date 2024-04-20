using System;
using NPC.SentenceNodes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace NPC
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private UDictionary<string, Narrator> narrators;
        [SerializeField] private SentenceGraph graph;
        [Space]
        public UnityEvent<string> onSentenceStart;
        public UnityEvent<string> onSentenceEnd;
        public UnityEvent onDialogueEnd;

        private SentenceData _currentData;
        private Narrator _currentNarrator;
        private Sentence _current;
        private double _time;
        private bool _wait = true;

        public bool IsStarted => _current != null;
        public bool IsPlaying { get; private set; }


        public void StartDialogue()
        {
            _current = SentenceGraph.Clone(graph.root);
            SwitchUpdate();
            PlayDialogue();
        }
        public void StartDialogue(string rootName)
        {
            _current = SentenceGraph.Clone(graph.roots.Find(r => r.RootName == rootName));
            SwitchUpdate();
            PlayDialogue();
        }

        public void PlayDialogue()
        {
            IsPlaying = true;
        }
        public void PauseDialogue()
        {
            IsPlaying = false;
            _currentNarrator?.Clear();
        }

        public void StopDialogue()
        {
            _current = null;
        }

        public string GetCurrentNarratorName()
        {
            return _currentData?.narrator;
        }

        public void Update()
        {
            if (_current.IsUnityNull() || !IsPlaying) return;
            if (_currentData == null)
            {
                CancelInvoke(nameof(GoToNext));
                GoToNext();
                return;
            }

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
            onSentenceEnd.Invoke(_current.tag);
            _currentNarrator?.Clear();
            _current = _current.GetNext();
            SwitchUpdate();
        }

        private void SwitchUpdate()
        {
            if (_current.IsUnityNull())
            {
                onDialogueEnd.Invoke();
                return;
            }
            _current.GetState(this);
            _currentData = _current.GetSentence(PlayerPrefs.GetString("Language"));
            _wait = false;
            _time = 0;
            if (_currentData.IsUnityNull()) return;
            _currentNarrator = narrators.TryGetValue(_currentData.narrator, out var narrator) ? narrator : narrators.Values[0];
            onSentenceStart.Invoke(_current.tag);
        }
    }
}