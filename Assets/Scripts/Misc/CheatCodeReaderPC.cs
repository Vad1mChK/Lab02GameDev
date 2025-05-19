using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Misc
{
    public class CheatCodeReaderPC : MonoBehaviour
    {
        [Serializable]
        private class CheatCode
        {
            public string name;
            public List<KeyCode> keySequence = new() { KeyCode.I, KeyCode.D, KeyCode.D, KeyCode.Q, KeyCode.D };
            public CheatAction action;
            [NonSerialized] public int currentProgress;
        }

        private enum CheatAction
        {
            InstantVictory,
            InstantDefeat,
            GetRandomItem,
        }

        [SerializeField] private string victorySceneName = "Victory";
        [SerializeField] private string defeatSceneName = "Defeat";
        [SerializeField] private List<ItemData> itemPool;
        [SerializeField] private Inventory.Inventory inventory;
        [SerializeField] private List<CheatCode> cheatCodes;
        
        [Header("Sounds")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip cheatUseSound;

        [Header("Misc")] [SerializeField] private float banTimeout = 5f;
        private void Update()
        {
            if (cheatCodes == null || cheatCodes.Count == 0) return;

            foreach (var cheat in cheatCodes)
            {
                if (cheat.keySequence.Count == 0) continue;

                // Reset progress on any other key press
                if (KeyUtils.Letters().AnyKeyDownFromSubset() && 
                    !Input.GetKeyDown(cheat.keySequence[cheat.currentProgress]))
                {
                    Debug.Log(
                        $"Cheat {cheat.name}: " +
                        $"wrong key pressed, reset progress",
                        this
                    );
                    cheat.currentProgress = 0;
                }
                
                if (cheat.currentProgress < cheat.keySequence.Count)
                {
                    if (Input.GetKeyDown(cheat.keySequence[cheat.currentProgress]))
                    {
                        Debug.Log(
                            $"Cheat {cheat.name}: " +
                            $"pressed key {cheat.keySequence[cheat.currentProgress]}, " + 
                            $"cheat progress {cheat.currentProgress}",
                            this
                        );
                        cheat.currentProgress++;
                        
                        if (cheat.currentProgress >= cheat.keySequence.Count)
                        {
                            Debug.Log(
                                $"Cheat {cheat.name}: " +
                                $"executed, reset progress",
                                this
                            );
                            ExecuteCheatAction(cheat.action);
                            cheat.currentProgress = 0;
                        }
                    }
                }
            }
        }

        public void AntiCheatBan()
        {
            StartCoroutine(AntiCheatRoutine());
        }

        private IEnumerator AntiCheatRoutine()
        {
            yield return new WaitForSeconds(banTimeout);

            SceneManager.LoadScene(defeatSceneName, LoadSceneMode.Single);
            yield break;
        }

        private void ExecuteCheatAction(CheatAction action)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(cheatUseSound);
            }
            switch (action)
            {
                case CheatAction.InstantVictory:
                    if (!string.IsNullOrEmpty(victorySceneName))
                        SceneManager.LoadScene(victorySceneName, LoadSceneMode.Single);
                    break;
                
                case CheatAction.InstantDefeat:
                    if (!string.IsNullOrEmpty(defeatSceneName))
                        SceneManager.LoadScene(defeatSceneName, LoadSceneMode.Single);
                    break;
                
                case CheatAction.GetRandomItem:
                    if (itemPool.Count > 0 && inventory != null)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, itemPool.Count);
                        inventory.Add(itemPool[randomIndex]);
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}