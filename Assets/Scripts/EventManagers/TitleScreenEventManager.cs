using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenEventManager : MonoBehaviour
{
    [Header("Window References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private List<GameObject> otherPanels;

    [Header("State Tracking")]
    [SerializeField] private bool settingsWindowVisible;
    [SerializeField] private bool aboutWindowVisible;

    [Header("Misc")]
    [SerializeField] private string roomSceneName = "SampleScene"; 

    public void PlayGame()
    {
        Debug.Log("Loading game scene");
        SceneManager.LoadScene(roomSceneName, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting application");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToggleSettingsWindowVisibility(bool visible)
    {
        // If no parameter passed, toggle current state
        settingsWindowVisible = visible;
        
        // Close other window if opening this one
        // if(settingsWindowVisible) ToggleAboutWindowVisibility(false);
        
        settingsPanel.SetActive(settingsWindowVisible);
        ToggleOtherPanelsVisibility(!aboutWindowVisible && !settingsWindowVisible);
    }

    public void ToggleAboutWindowVisibility(bool visible)
    {
        // If no parameter passed, toggle current state
        aboutWindowVisible = visible;
        
        // Close other window if opening this one
        // if(aboutWindowVisible) ToggleSettingsWindowVisibility(false);
        
        aboutPanel.SetActive(aboutWindowVisible);
        ToggleOtherPanelsVisibility(!aboutWindowVisible && !settingsWindowVisible);
    }

    private void ToggleOtherPanelsVisibility(bool visible)
    {
        otherPanels.ForEach(it => it.SetActive(visible));
    }
}