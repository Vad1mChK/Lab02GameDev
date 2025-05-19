using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Text
{
    public class StatelessTextChanger : TextChanger
    {
        [SerializeField] [Multiline] private string initialText = DefaultInitialText;

        private void Start()
        {
            textElement.SetText(initialText);
        }

        public override void SetText(string newText)
        {
            textElement.SetText(newText);
        }

        public override void ResetText()
        {
            textElement.SetText(initialText);
        }
    }
}