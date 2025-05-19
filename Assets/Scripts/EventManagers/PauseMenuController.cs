using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPauseEvent;
    [SerializeField] private UnityEvent OnResumeEvent;
    [SerializeField] private bool paused = false;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPaused(bool newPaused)
    {
        paused = newPaused;
        if (paused)
        {
            OnPauseEvent?.Invoke();
        }
        else
        {
            OnResumeEvent?.Invoke();
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScreenScene", LoadSceneMode.Single);
    }
}
