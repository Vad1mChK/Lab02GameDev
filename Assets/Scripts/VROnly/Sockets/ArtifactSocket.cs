using Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VROnly.Items;

namespace VROnly.Sockets
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public class ArtifactSocket : MonoBehaviour
    {
        [SerializeField] private ItemType requiredItemType;
        [SerializeField] private InventoryItemEvent onItemAttachedEvent;
        [SerializeField] private InventoryItemEvent onItemReleasedEvent;

        private XRSocketInteractor _socketInteractor;
        private HeldItem _currentItem;

        private void Awake()
        {
            _socketInteractor = GetComponent<XRSocketInteractor>();
        }

        private void OnEnable()
        {
            _socketInteractor.selectEntered.AddListener(OnItemAttached);
            _socketInteractor.selectExited.AddListener(OnItemReleased);
        }

        private void OnDisable()
        {
            _socketInteractor.selectEntered.RemoveListener(OnItemAttached);
            _socketInteractor.selectExited.RemoveListener(OnItemReleased);
        }

        private void OnItemAttached(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.TryGetComponent<HeldItem>(out var heldItem))
            {
                if ((heldItem.Type & requiredItemType) != 0)
                {
                    _currentItem = heldItem;
                    onItemAttachedEvent?.Invoke(heldItem.Data);
                }
                else
                {
                    // Reject invalid item
                    _socketInteractor.interactionManager.SelectExit(
                        _socketInteractor, 
                        args.interactableObject
                    );
                }
            }
        }

        private void OnItemReleased(SelectExitEventArgs args)
        {
            if (_currentItem != null && args.interactableObject.transform == _currentItem.transform)
            {
                onItemReleasedEvent?.Invoke(_currentItem.Data);
                _currentItem = null;
            }
        }
    }
}