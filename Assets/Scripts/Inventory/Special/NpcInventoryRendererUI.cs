using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.UI;
// for NpcCharacterController

// for ItemType

namespace Inventory.Special
{
    public class NpcInventoryRendererUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NpcCharacterController npc;

        [SerializeField] private Canvas inventoryCanvas;
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private GameObject       iconPrefab;    // prefab with an Image on root

        [System.Serializable]
        public class IconMapping
        {
            public ItemType type;   // e.g. ItemType.Spear
            public Sprite   icon;   // sprite to show for that type
        }

        [Header("Icon Setup")]
        [SerializeField] private List<IconMapping> iconMappings;

        // keep track of instantiated icons so we can clear them
        private readonly List<GameObject> _spawnedIcons = new List<GameObject>();
        private bool _visible = true;

        public bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        private void OnEnable()
        {
            // subscribe and build initial
            npc.OnUpdateObtainedArtifactsEvent.AddListener(RebuildUI);
            RebuildUI();
        }

        private void OnDisable()
        {
            npc.OnUpdateObtainedArtifactsEvent.RemoveListener(RebuildUI);
        }

        public void RebuildUI()
        {
            // 1) clear previous icons
            foreach (var go in _spawnedIcons)
                Destroy(go);
            _spawnedIcons.Clear();

            // 2) for each mapping, if that flag is set, spawn an icon
            foreach (var map in iconMappings)
            {
                if ((npc.obtainedArtifacts & map.type) != 0)
                {
                    var go = Instantiate(iconPrefab, iconContainer);
                    if (go.TryGetComponent<Image>(out var img))
                        img.sprite = map.icon;
                    _spawnedIcons.Add(go);
                }
            }
        }

        private void SetVisible(bool visible)
        {
            this.inventoryCanvas.enabled = visible;
            this.enabled = visible;
            _visible = visible;
        }
    }
}