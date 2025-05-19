using System;
using UnityEngine;

namespace Misc
{
    public class ItemUseController: MonoBehaviour
    {
        [SerializeField] private Inventory.Inventory inventory;
        [SerializeField] private KeyCode useKey = KeyCode.E;

        void Awake()
        {
            if (inventory == null)
            {
                inventory = GetComponent<Inventory.Inventory>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(useKey))
            {
                inventory.UseCurrent();
            }
        }
    }
}