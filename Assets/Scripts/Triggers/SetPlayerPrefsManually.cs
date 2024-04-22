using System;
using Plugins.DialogueSystem.Scripts.Utils;
using UnityEngine;

namespace Triggers
{
    public class SetPlayerPrefsManually : MonoBehaviour
    {
        [SerializeField] private UDictionary<string, int> intPrefs;
        [SerializeField] private UDictionary<string, float> floatPrefs;
        [SerializeField] private UDictionary<string, string> stringPrefs;

        private void Start()
        {
            foreach (var pair in intPrefs)
                PlayerPrefs.SetInt(pair.Key, pair.Value);
            foreach (var pair in floatPrefs)
                PlayerPrefs.SetFloat(pair.Key, pair.Value);
            foreach (var pair in stringPrefs)
                PlayerPrefs.SetString(pair.Key, pair.Value);

            Destroy(this);
        }
    }
}