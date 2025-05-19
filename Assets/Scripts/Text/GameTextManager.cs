using System;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Text
{
    public class GameTextManager: MonoBehaviour
    {
        [FormerlySerializedAs("informationTextElement")] 
        [SerializeField] private TaggedStackTextChanger informationTextChanger;
        [SerializeField] private StatelessTextChanger timerTextElement;
        [SerializeField] private uint timerPrecision;
        public string DebugInfo => informationTextChanger.DebugInfo;

        public void PushInformationText(string newText)
        {
            informationTextChanger.PushText(newText);
        }

        public void PushInformationText(string newText, string newTag)
        {
            informationTextChanger.PushText(newText: newText, newTag: newTag);
        }
        
        [Tooltip("Tag and text are separated by `::`")]
        public void PushInformationTextDelimited(string newTextAndTag)
        {
            const string delimiter = "::";
            var parts = newTextAndTag.Split(
                delimiter, 
                options: StringSplitOptions.RemoveEmptyEntries
            );
            if (parts.Length != 2)
            {
                Debug.LogWarning(
                    $"PushInformationTextDelimited: Expected 2 parts, got {parts.Length}", 
                    this
                    );
                return;
            }

            string newTag = parts[0].Trim(), newText = parts[1].Trim();
            
            PushInformationText(newText: newText, newTag: newTag);
        }
        
        public void PopInformationText()
        {
            informationTextChanger.PopText();
        }

        public void RemoveInformationTextByTag(string tagToRemove)
        {
            informationTextChanger.RemoveAllByTag(tagToRemove);
        }

        public void ResetInformationText()
        {
            informationTextChanger.ResetText();
        }

        public void SetTimerValue(float seconds)
        {
            timerTextElement.SetText(FormatTime(seconds, timerPrecision));
        }

        private string FormatTime(float seconds, uint precision = 0)
        {
            var stringBuilder = new StringBuilder();
            int wholeSeconds = Mathf.FloorToInt(seconds);
            float fractionalPart = seconds - wholeSeconds; // Correct fractional calculation
    
            int hours = wholeSeconds / 3600;
            int minutes = (wholeSeconds % 3600) / 60;
            int secondsOfMinute = wholeSeconds % 60;

            // Add hours only if they exist
            if (hours > 0)
            {
                stringBuilder.Append($"{hours:D2}:");
            }

            // Always show minutes and seconds
            stringBuilder.Append($"{minutes:D2}:{secondsOfMinute:D2}");

            // Add fractional part if precision requested
            if (precision > 0)
            {
                // Calculate fractional seconds and format with requested precision
                stringBuilder.Append($".{Mathf.FloorToInt(fractionalPart * Mathf.Pow(10, precision))}:D{precision}");
            }

            return stringBuilder.ToString();
        }
    }
}