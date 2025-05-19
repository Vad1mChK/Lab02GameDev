using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class GameUIPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameUIPanelState uiPanelState = GameUIPanelState.Inventory;
    [SerializeField] private GameObject inventoryUiGroup;
    [SerializeField] private GameObject safePanelUiGroup;
    [SerializeField] private GameObject pauseMenuUiGroup;
    [SerializeField] private bool paused;
    
    private Stack<GameUIPanelState> _stateStack;
    private Dictionary<GameUIPanelState, GameObject> _canvasGroups;
    
    public enum GameUIPanelState
    {
        Inventory,
        SafePanel,
        PauseMenu
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvasGroups = new Dictionary<GameUIPanelState, GameObject> {
            { GameUIPanelState.Inventory, inventoryUiGroup },
            { GameUIPanelState.SafePanel, safePanelUiGroup },
            { GameUIPanelState.PauseMenu, pauseMenuUiGroup }
        };
        
        _stateStack = new Stack<GameUIPanelState>();
        SwitchToPanel(uiPanelState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToInventoryPanel() => SwitchToPanel(GameUIPanelState.Inventory);

    public void SwitchToSafePanel() => SwitchToPanel(GameUIPanelState.SafePanel);

    public void SwitchToPauseMenu() => SwitchToPanel(GameUIPanelState.PauseMenu);

    public void SwitchToPanel(GameUIPanelState newState, bool doPush = true)
    {
        if (doPush) _stateStack.Push(newState);
        
        foreach (var state in _canvasGroups.Keys)
        {
            _canvasGroups[state].SetActive(state == newState);
        }
        
        Debug.Log(_stateStack.PrettyPrint(), this);
    }

    public void SetPausedState(bool newPaused)
    {
        paused = newPaused;
    
        if (paused)
        {
            _stateStack.Push(GameUIPanelState.PauseMenu);
        }
        else
        {
            while (_stateStack.Count > 0 && _stateStack.Peek() == GameUIPanelState.PauseMenu)
            {
                _stateStack.Pop();
            }
        }
    
        SwitchToPanel(_stateStack.Count > 0 ? _stateStack.Peek() : GameUIPanelState.Inventory, false);
    }

    public void TogglePausedState()
    {
        SetPausedState(!paused);
    }
}
