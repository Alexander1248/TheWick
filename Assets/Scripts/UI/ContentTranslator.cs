using System;
using Plugins.DialogueSystem.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ContentTranslator : MonoBehaviour
    {
        
        [SerializeField] private TMP_Text text;
        [Space]
        [SerializeField] private string localePlayerPref;
        [SerializeField] private string defaultLocale;
        [SerializeField] private UDictionary<string, string> localizations = new();
        private void Update()
        {
            var locale = PlayerPrefs.HasKey(localePlayerPref) ? PlayerPrefs.GetString(localePlayerPref) : defaultLocale;
            if (localizations.TryGetValue(locale, out var text)) this.text.text = text;
            else this.text.text = localizations.Values.Count > 0 ? localizations.Values[0] : "";
        }
    }
}