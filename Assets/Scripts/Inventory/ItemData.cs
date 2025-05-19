using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Item/BaseItem", fileName = "Item", order = 0)]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public ItemType itemType;
        
        [Header("Audio")]
        public AudioClip onUseSoundEffect;
        public float pitchVariation;
        
        [Header("Prefabs")]
        [Tooltip("Prefab spawned as an artifact ('regular' scene object)")]
        public GameObject artifactPrefab;

        [Tooltip("Prefab used for pickups (pooled)")]
        public GameObject pickupPrefab;

        [Tooltip("Prefab used for holding the item in hand")]
        public GameObject inHandPrefab;

        [Tooltip("Whether the item is pickable from the start")]
        [Header("Misc")]
        public bool startPickable = true;

        public bool reusable = true;

        public virtual bool Use(GameObject user)
        {
            Debug.Log($"Item {itemName} used by {user};");
            if (onUseSoundEffect != null && user.TryGetComponent<AudioSource>(out var audioSource))
            {
                var originalPitch = audioSource.pitch;
                audioSource.pitch = 1 + (Random.value - 0.5f) * pitchVariation;
                audioSource.PlayOneShot(onUseSoundEffect);
                // audioSource.pitch = originalPitch;
            }
            return false;
        }
    }
}