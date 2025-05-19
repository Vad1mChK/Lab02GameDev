using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class InteractionDetector : MonoBehaviour
    {
        [Tooltip("Only colliders with this tag will be tracked")] [SerializeField]
        private string targetTag = "NPC";

        // All NPCs currently in range
        [SerializeField] private List<NpcCharacterController> _nearbyNpcs = new();

        // private void Reset()
        // {
        //     // auto‐configure a SphereCollider as trigger
        //     var col = GetComponent<Collider>();
        //     col.isTrigger = true;
        //     if (!(col is SphereCollider))
        //     {
        //         DestroyImmediate(col);
        //         gameObject.AddComponent<SphereCollider>().isTrigger = true;
        //     }
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(targetTag)) return;

            var npc = other.GetComponent<NpcCharacterController>();
            if (npc != null && !_nearbyNpcs.Contains(npc))
                _nearbyNpcs.Add(npc);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(targetTag)) return;

            var npc = other.GetComponent<NpcCharacterController>();
            if (npc != null)
                _nearbyNpcs.Remove(npc);
        }

        /// <summary>
        ///     Returns the closest NPC in range (or null if none).
        /// </summary>
        public NpcCharacterController GetClosestNpc()
        {
            NpcCharacterController best = null;
            var bestDistSq = float.MaxValue;
            var myPos = transform.position;

            foreach (var npc in _nearbyNpcs)
            {
                // skip NPCs that are out of range because maybe collider shapes differ
                var d2 = (npc.transform.position - myPos).sqrMagnitude;
                if (d2 < bestDistSq)
                {
                    bestDistSq = d2;
                    best = npc;
                }
            }

            return best;
        }
    }
}