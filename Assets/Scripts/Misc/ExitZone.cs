using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class ExitZone : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string victorySceneName;
        [SerializeField] private float teleportDelay = 1f;

        [Header("Events")]
        [SerializeField] private UnityEvent onAttemptStart;    // When teleport initiation occurs
        [SerializeField] private UnityEvent onAttemptCancel;   // When canceled by leaving zone
        [SerializeField] private UnityEvent onAttemptComplete; // When countdown finishes successfully
        [SerializeField] private UnityEvent<float> onProgressUpdate; // Progress from 0-1

        private bool _playerInZone;
        private Coroutine _currentAttempt;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("The player has entered the exit zone, canceling exit attempt...");
                _playerInZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("The player has left the exit zone, canceling exit attempt...");
                _playerInZone = false;
                CancelAttempt();
            }
        }

        // Call this method from your input system (e.g., button press)
        public void StartTeleportAttempt()
        {
            if (!_playerInZone || _currentAttempt != null) return;

            _currentAttempt = StartCoroutine(TeleportCountdown());
            onAttemptStart?.Invoke();
        }

        private IEnumerator TeleportCountdown()
        {
            float timer = 0f;
            
            while (timer < teleportDelay && _playerInZone)
            {
                timer += Time.deltaTime;
                onProgressUpdate?.Invoke(timer / teleportDelay);
                yield return null;
            }

            if (_playerInZone)
            {
                onAttemptComplete?.Invoke();
                AttemptTeleport();
            }
            else
            {
                onAttemptCancel?.Invoke();
            }

            _currentAttempt = null;
        }

        private void CancelAttempt()
        {
            if (_currentAttempt != null)
            {
                StopCoroutine(_currentAttempt);
                _currentAttempt = null;
                onAttemptCancel?.Invoke();
            }
        }

        private void AttemptTeleport()
        {
            if (!string.IsNullOrEmpty(victorySceneName))
            {
                SceneManager.LoadScene(victorySceneName, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogWarning("Victory scene name not set!", this);
            }
        }

        private void OnDisable()
        {
            CancelAttempt();
        }
    }
}