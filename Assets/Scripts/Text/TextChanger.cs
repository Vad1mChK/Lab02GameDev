using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace Text
{
    public abstract class TextChanger: MonoBehaviour
    {
        protected const string DefaultInitialText = "";
        [SerializeField] protected TMP_Text textElement;

        public string DebugInfo => GetDebugInfo();
        [SerializeField] protected string debugInfo;
        
        public abstract void SetText(string text);

        public abstract void ResetText();

        protected virtual string GetDebugInfo()
        {
            Debug.LogWarning("Debug info for this text changer is unavailable");
            return "";
        }
    }
}