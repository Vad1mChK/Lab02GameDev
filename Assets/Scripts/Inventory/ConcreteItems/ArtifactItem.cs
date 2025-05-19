using System;
using Misc;
using UnityEngine;

namespace Inventory.ConcreteItems
{
    [CreateAssetMenu(menuName = "Inventory/Item/Artifact", fileName = "Artifact", order = 0)]
    public class ArtifactItem : ItemData
    {
        public override bool Use(GameObject user)
        {
            base.Use(user);
            if (user.TryGetComponent<InteractionDetector>(out var detector))
            {
                var npc = detector.GetClosestNpc();
                if (npc == null) return false;

                Debug.Log("Found NPC");
                
                if (!npc.Interactive)
                {
                    return false;
                }
                
                Debug.Log("NPC is interactive");

                if ((itemType & npc.requiredArtifacts) == 0)
                {
                    Debug.Log($"NPC does not require the artifact {itemType}. They need {npc.requiredArtifacts}");
                    return false;
                }
                
                Debug.Log("NPC requires this artifact");

                npc.OnReceiveArtifact(this);
                Debug.Log($"Delivered artifact {itemName} to NPC");
                return true;
            }

            return false;
        }
    }
}