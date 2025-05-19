using Inventory;
using Inventory.ConcreteItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VROnly.Items;

namespace VROnly.Sockets
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public class KeySocket : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string correctDoorId;
        [SerializeField] private XRSocketInteractor socketInteractor;

        [Header("Events")]
        [SerializeField] private UnityEvent<ItemData> onKeyAttachEvent;
        [SerializeField] private UnityEvent<ItemData> onKeyReleaseEvent;
        [SerializeField] private UnityEvent<ItemData> onCorrectKeyAttachEvent;
        [SerializeField] private UnityEvent<ItemData> onWrongKeyAttachedEvent;

        private void Awake()
        {
            if (socketInteractor == null)
            {
                socketInteractor = GetComponent<XRSocketInteractor>();
            }
        }

        private void OnEnable()
        {
            socketInteractor.selectEntered.AddListener(OnSelectEntered);
            socketInteractor.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
            socketInteractor.selectExited.RemoveListener(OnSelectExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.TryGetComponent<HeldItem>(out var heldItem))
            {
                ProcessKeyAttachment(heldItem);
            }
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            if (args.interactableObject.transform.TryGetComponent<HeldItem>(out var heldItem))
            {
                ProcessKeyRelease(heldItem);
            }
        }

        private void ProcessKeyAttachment(HeldItem heldItem)
        {
            if (heldItem.Data is KeyItem keyItem)
            {
                onKeyAttachEvent?.Invoke(heldItem.Data);
                
                Debug.Log($"KeySocket: required door id {correctDoorId}, actual {keyItem.doorId}");

                if (keyItem.doorId == correctDoorId)
                {
                    onCorrectKeyAttachEvent?.Invoke(heldItem.Data);
                }
                else
                {
                    onWrongKeyAttachedEvent?.Invoke(heldItem.Data);
                }
            }
        }

        private void ProcessKeyRelease(HeldItem heldItem)
        {
            if (heldItem.Data is KeyItem)
            {
                onKeyReleaseEvent?.Invoke(heldItem.Data);
            }
        }
    }
}