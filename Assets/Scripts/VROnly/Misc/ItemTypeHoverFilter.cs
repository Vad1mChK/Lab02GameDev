﻿using Inventory;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VROnly.Items;

namespace VROnly.Misc
{
    public class ItemTypeHoverFilter: MonoBehaviour, IXRHoverFilter
    {
        [SerializeField] private ItemType allowedItemType = ItemType.None;

        public bool canProcess => isActiveAndEnabled;

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            if (interactor is not XRSocketInteractor) return false;

            var baseInteractable = interactable as XRBaseInteractable;
            if (baseInteractable == null) return false;

            if (!baseInteractable.TryGetComponent<HeldItem>(out var heldItem)) return false;
            // Debug.Log(
            //     $"ItemTypeHoverFilter: required {allowedItemType}, actual {heldItem.Type}", 
            //     this
            // );
            return ((heldItem.Type & allowedItemType) != 0);

        }
    }
}