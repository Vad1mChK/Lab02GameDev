using UnityEngine;

namespace VR.Tutorial
{
    public class ArmchairInteract: MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;

        public void Jump()
        {
            rigidbody.AddForce(new Vector3(0, 10f, 0), ForceMode.Impulse);
        }
    }
}