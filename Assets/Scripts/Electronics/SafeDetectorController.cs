using System;
using UnityEngine;
using UnityEngine.Events;

public class SafeDetectorController : MonoBehaviour
{
    [SerializeField] private KeyCode keyToActivate = KeyCode.G;
    [SerializeField] private UnityEvent OnTriggerEnterEvent;
    [SerializeField] private UnityEvent OnTriggerExitEvent;
    [SerializeField] private UnityEvent OnSafePanelOpenEvent;
    [SerializeField] private UnityEvent OnSafePanelCloseEvent;
    private bool _interactive = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToActivate) && _interactive)
        {
            OnSafePanelOpenEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactive = true;
            OnTriggerEnterEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interactive = false;
            OnTriggerExitEvent?.Invoke();
            OnSafePanelCloseEvent?.Invoke();
        }
    }
}
