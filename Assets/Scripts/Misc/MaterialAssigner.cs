using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public class MaterialAssigner: MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> meshRenderers;

        public void AssignToAll(Material material)
        {
            meshRenderers.ForEach(mr =>
                mr.material = material
            );
        }
    }
}