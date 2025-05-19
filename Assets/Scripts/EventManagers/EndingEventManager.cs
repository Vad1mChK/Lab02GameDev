using System;
using System.IO;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventManagers
{
    public class EndingEventManager: MonoBehaviour
    {
        [SerializeField] private bool displayTimeElapsed;
        [SerializeField] private string titleSceneName = "TitleScreenScene";
        [SerializeField] [CanBeNull] private TMP_Text timeElapsedText; 

        private void Start()
        {
            if (displayTimeElapsed && timeElapsedText == null)
            {
                throw new NullReferenceException("Timer text element is null");
            }
        }
        
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(titleSceneName, LoadSceneMode.Single);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}