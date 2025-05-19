using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SafePanelController : MonoBehaviour
{
    [SerializeField] private string secretCode = "1234";
    [SerializeField] private uint maxCodeLength = 8;
    [SerializeField] private Image safePanelScreenUiElement;
    [SerializeField] private Sprite safePanelScreenSpriteForIdle;
    [SerializeField] private Sprite safePanelScreenSpriteForSuccess;
    [SerializeField] private Sprite safePanelScreenSpriteForFailure;
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private float validateDelaySeconds = 1;
    [SerializeField] private SafePanelState state;

    private readonly Dictionary<KeyCode, char> _keyToDigitMapping = new Dictionary<KeyCode, char>()
    {
        {KeyCode.Alpha0, '0'}, {KeyCode.Keypad0, '0'},
        {KeyCode.Alpha1, '1'}, {KeyCode.Keypad1, '1'},
        {KeyCode.Alpha2, '2'}, {KeyCode.Keypad2, '2'},
        {KeyCode.Alpha3, '3'}, {KeyCode.Keypad3, '3'},
        {KeyCode.Alpha4, '4'}, {KeyCode.Keypad4, '4'},
        {KeyCode.Alpha5, '5'}, {KeyCode.Keypad5, '5'},
        {KeyCode.Alpha6, '6'}, {KeyCode.Keypad6, '6'},
        {KeyCode.Alpha7, '7'}, {KeyCode.Keypad7, '7'},
        {KeyCode.Alpha8, '8'}, {KeyCode.Keypad8, '8'},
        {KeyCode.Alpha9, '9'}, {KeyCode.Keypad9, '9'}
    };

    private StringBuilder _codeBuilder;
    private Coroutine _validationRoutine;

    public UnityEvent OnInputEvent;
    public UnityEvent OnSuccessEvent;
    public UnityEvent OnFailureEvent;
    public UnityEvent OnCloseUIEvent;

    private enum SafePanelState
    {
        Idle,
        Validating,
        ValidateSuccess,
        ValidateFailure
    }

    void Start()
    {
        _codeBuilder = new StringBuilder();
        UpdateDisplay();
    }

    void Update()
    {
        if (state != SafePanelState.Idle) return;

        HandleKeyboardInput();
        CheckForAutoValidation();
    }

    private void HandleKeyboardInput()
    {
        foreach (var kvp in _keyToDigitMapping)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                AppendDigit(kvp.Value);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            OnClear();
        }
    }

    private void CheckForAutoValidation()
    {
        if (_codeBuilder.Length == secretCode.Length && _validationRoutine == null)
        {
            _validationRoutine = StartCoroutine(ValidationProcess());
        }
    }

    private void AppendDigit(char digit)
    {
        if (_codeBuilder.Length >= secretCode.Length || _codeBuilder.Length >= maxCodeLength) return;
        
        _codeBuilder.Append(digit);
        OnInputEvent?.Invoke();
        UpdateDisplay();
    }

    public void OnDigitAppend(string digits)
    {
        if (state != SafePanelState.Idle || digits.Length != 1) return;
        AppendDigit(digits[0]);
    }

    public void OnClear()
    {
        if (state == SafePanelState.Idle)
        {
            _codeBuilder.Clear();
            OnInputEvent?.Invoke();
            UpdateDisplay();
        }
    }

    private IEnumerator ValidationProcess()
    {
        state = SafePanelState.Validating;
        UpdateDisplay();

        yield return new WaitForSeconds(validateDelaySeconds);

        bool isValid = Validate();
        state = isValid ? SafePanelState.ValidateSuccess : SafePanelState.ValidateFailure;
        if (isValid)
        {
            OnSuccessEvent?.Invoke();
        }
        else
        {
            OnFailureEvent?.Invoke();
        }
        UpdateDisplay();

        if (isValid)
        {
            // Trigger safe unlock logic here
            yield return new WaitForSeconds(1);
            OnCloseUIEvent?.Invoke();
        }
        else
        {
            yield return new WaitForSeconds(1);
            ResetPanel();
        }

        _validationRoutine = null;
    }

    private void ResetPanel()
    {
        _codeBuilder.Clear();
        state = SafePanelState.Idle;
        UpdateDisplay();
    }

    private bool Validate()
    {
        return _codeBuilder.ToString() == secretCode;
    }

    private void UpdateDisplay()
    {
        safePanelScreenUiElement.sprite = state switch
        {
            SafePanelState.Idle => safePanelScreenSpriteForIdle,
            SafePanelState.ValidateSuccess => safePanelScreenSpriteForSuccess,
            SafePanelState.ValidateFailure => safePanelScreenSpriteForFailure,
            _ => safePanelScreenSpriteForIdle
        };
        
        outputText.text = state switch
        {
            SafePanelState.Validating => "CHECKING",
            SafePanelState.ValidateSuccess => "CORRECT",
            SafePanelState.ValidateFailure => "WRONG",
            _ => _codeBuilder.ToString()
        };
    }
}