using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Text
{
    public class TaggedStackTextChanger: TextChanger
    {
        private const string DefaultTag = "";
        private Stack<TextEntry> _textStack = new Stack<TextEntry>();
        
        [SerializeField] private string initialText = DefaultInitialText;
        [SerializeField] private string initialTag = DefaultTag;
        
        public List<TextEntry> TextEntries => _textStack.ToList();
        public string CurrentText => _textStack.Count > 0 ? _textStack.Peek().text : "";
        public string CurrentTag => _textStack.Count > 0 ? _textStack.Peek().tag : "";
        
        [Serializable]
        public struct TextEntry
        {
            public string text;
            public string tag;
        }
        
        private static class TextColorizerUtil
        {
            /// <summary>
            /// computes hashcode on text
            /// gets r,g,b components using a bitwise mask
            /// then for each channel performs 0.5 + 0.5 * channel
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static Color DetermineColorForTag(string tag, float whiteStrength = 0.5f)
            {
                // Handle null/empty case
                if (string.IsNullOrEmpty(tag))
                {
                    return new Color(0.8f, 0.8f, 0.8f); // Neutral gray
                }

                // Compute hash code
                int hash = tag.GetHashCode();
        
                // Extract RGB components using bitwise operations
                byte r = (byte)((hash >> 16) & 0xFF); // First 8 bits after 16-bit shift
                byte g = (byte)((hash >> 8) & 0xFF);  // Next 8 bits
                byte b = (byte)(hash & 0xFF);         // Last 8 bits

                // Convert to 0.5-1.0 range (good for dark backgrounds)
                return new Color(
                    whiteStrength + (1 - whiteStrength) * (r / 255f),
                    whiteStrength + (1 - whiteStrength) * (g / 255f),
                    whiteStrength + (1 - whiteStrength) * (b / 255f)
                );
            }
        }

        private void Start()
        {
            PushText(newText: initialText, newTag: initialTag);
        }

        private void Update()
        {
            debugInfo = GetDebugInfo();
        }

        public override void SetText(string text)
        {
            PushText(text);
        }

        public override void ResetText()
        {
            _textStack.Clear();
            PushText(newText: initialText, newTag: initialTag);
        }
        
        public void PushText(string newText, string newTag = DefaultTag)
        {
            _textStack.Push(new TextEntry { text = newText, tag = newTag });
            UpdateDisplay();
        }

        public void PopText()
        {
            if (_textStack.Count > 1)
            {
                _textStack.Pop();
                UpdateDisplay();
            }
        }
        
        public void RemoveAllByTag(string targetTag)
        {
            List<TextEntry> tempList = new List<TextEntry>();

            // Empty the stack
            while (_textStack.Count > 0)
            {
                tempList.Add(_textStack.Pop());
            }

            // Filter out entries with target tag
            tempList.RemoveAll(entry => entry.tag == targetTag);

            // Restore remaining entries in original order
            tempList.Reverse();
            foreach (var entry in tempList)
            {
                _textStack.Push(entry);
            }

            // Ensure at least one entry remains
            if (_textStack.Count == 0)
            {
                PushText(initialText, DefaultTag);
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            textElement.text = _textStack.Peek().text;
            textElement.color = TextColorizerUtil.DetermineColorForTag(_textStack.Peek().tag);
        }
        
        protected override string GetDebugInfo()
        {
            return  $"stack size: {_textStack.Count}\n" +
                    $"stack top text: {_textStack.Peek().text}\n" + 
                    $"stack top tag: {_textStack.Peek().tag}";
        }
    }
}