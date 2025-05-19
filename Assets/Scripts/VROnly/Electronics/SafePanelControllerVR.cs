using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VROnly.Electronics
{
    public class SafePanelControllerVR : MonoBehaviour
    {
        public enum SafePanelState
        {
            Idle,
            Checking,
            Correct,
            Wrong
        }
        
        [System.Serializable]
        public struct SafePanelStateMapping
        {
            public SafePanelState state;
            public Sprite sprite;
        }
        
        private StringBuilder _codeBuilder = new StringBuilder();
        private Dictionary<SafePanelState, Sprite> _screenSprites;
        private Coroutine _checkingCoroutine;
        
        [SerializeField] private int maxDigitCount = 12;
        [SerializeField] private SafePanelState state = SafePanelState.Idle;
        [SerializeField] private string correctCode;
        [Header("Strings")]
        [SerializeField] private string checkingStateText = "Checking...";
        [SerializeField] private string correctStateText = "Correct";
        [SerializeField] private string wrongStateText = "Wrong";
        [Header("UI References")]
        [SerializeField] private Image screenImage;
        [SerializeField] private List<SafePanelStateMapping> screenSprites;
        [SerializeField] private TMP_Text textElement;
        [Header("Events")] 
        [SerializeField] private UnityEvent onCorrectCodeEntered;
        [SerializeField] private UnityEvent onWrongCodeEntered;
        [SerializeField] private UnityEvent onCorrectResultDisplayEnded;
        [SerializeField] private UnityEvent onWrongResultDisplayEnded;
        [Header("Timing")]
        [SerializeField] private float checkingTimeout = 1f;
        [SerializeField] private float resultDisplayTime = 1.5f;

        private void Start()
        {
            _screenSprites = screenSprites
                .ToDictionary(
                    mapping => mapping.state,
                    mapping => mapping.sprite
                );
            UpdateVisualization();
        }

        public void AppendDigitToCode(string digit)
        {
            if (state != SafePanelState.Idle) return;
            if (_codeBuilder.Length + digit.Length <= maxDigitCount)
            {
                _codeBuilder.Append(digit);
                UpdateVisualization();
                
                if (_codeBuilder.Length == maxDigitCount ||
                    _codeBuilder.ToString() == correctCode)
                {
                    StartChecking();
                }
            }
        }

        public void ClearCode()
        {
            if (state == SafePanelState.Checking) return;
            _codeBuilder.Clear();
            state = SafePanelState.Idle;
            UpdateVisualization();
        }

        private void StartChecking()
        {
            if (_checkingCoroutine != null)
            {
                StopCoroutine(_checkingCoroutine);
            }
            _checkingCoroutine = StartCoroutine(CheckCodeCoroutine());
        }

        private IEnumerator CheckCodeCoroutine()
        {
            // Start checking state
            state = SafePanelState.Checking;
            UpdateVisualization();
            
            // Simulate checking delay
            yield return new WaitForSeconds(checkingTimeout);
            
            // Verify code
            if (_codeBuilder.ToString() == correctCode)
            {
                state = SafePanelState.Correct;
                onCorrectCodeEntered.Invoke();
            }
            else
            {
                state = SafePanelState.Wrong;
                onWrongCodeEntered.Invoke();
            }
            UpdateVisualization();
            
            // Show result state
            yield return new WaitForSeconds(resultDisplayTime);

            switch (state)
            {
                case SafePanelState.Correct:
                    onCorrectResultDisplayEnded?.Invoke();
                    break;
                case SafePanelState.Wrong:
                    onWrongResultDisplayEnded?.Invoke();
                    break;
            }
            
            // Reset to idle state
            state = SafePanelState.Idle;
            _codeBuilder.Clear();
            UpdateVisualization();
        }

        private void UpdateVisualization()
        {
            // Update text display
            if (textElement != null)
            {
                textElement.text = state switch
                {
                    SafePanelState.Checking => checkingStateText,
                    SafePanelState.Correct => correctStateText,
                    SafePanelState.Wrong => wrongStateText,
                    _ => _codeBuilder.ToString()
                };
            }

            // Update screen image
            if (screenImage != null && _screenSprites.TryGetValue(state, out var sprite))
            {
                screenImage.sprite = sprite;
            }
        }
    }
}