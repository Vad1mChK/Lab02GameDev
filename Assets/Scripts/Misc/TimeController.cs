using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Time Controls")]
    [Range(0f, 2f)] public float gameSpeed = 1f;
    public bool isPaused;

    void Update()
    {
        // Standard time control
        Time.timeScale = isPaused ? 0 : gameSpeed;

        // Toggle pause with P key
        if(Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        // Slow motion while holding Left Shift
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = 0.2f;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        AudioListener.pause = isPaused; // Pause audio too
    }

    public void SetGameSpeed(float speed)
    {
        gameSpeed = Mathf.Clamp(speed, 0f, 2f);
    }
}