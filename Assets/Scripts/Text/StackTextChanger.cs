using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Text
{
    public class StackTextChanger : TextChanger
    {
        [SerializeField] [Multiline] private string initialText = DefaultInitialText;
        private Stack<string> _textsStack;

        private void Start()
        {
            _textsStack = new Stack<string>();
            PushText(initialText);
        }

        public override void SetText(string text)
        {
            PushText(text);
        }

        public override void ResetText()
        {
            _textsStack.Clear();
            PushText(initialText);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void PushText(string newText = "venom")
        {
            _textsStack.Push(newText);
            textElement.SetText(newText);
        }

        public void PopText()
        {
            if (_textsStack.Count > 1) _textsStack.Pop();
            textElement.SetText(_textsStack.Peek());
        }

        protected override string GetDebugInfo()
        {
            return  $"stack size: {_textsStack.Count}\nstack top: {_textsStack.Peek()}";
        }
    }
}