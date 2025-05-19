using UnityEngine;

namespace Characters
{
    public class RiggedProtagonistController : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Key to be pressed for jump")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        [Header("Movement")]
        [SerializeField] private float topSpeed = 5f;
        [SerializeField] private float rotationSpeed = 150f;
        [SerializeField] private float jumpForce = 5f;
    
        [Header("Misc")]
        [SerializeField] private ProtagonistState protagonistState = ProtagonistState.Idle;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rigidbody;

        private bool isGrounded = true;
        
        private enum ProtagonistState
        {
            Idle,
            Walking,
            Jumping
        }
    
        void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // Handle rotation with left/right keys
            float horizontalInput = Input.GetAxis("Horizontal");
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(0f, (horizontalInput + mouseX) * rotationSpeed * Time.deltaTime, 0f);

            // Handle movement with up/down keys
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 movementDirection = transform.forward * verticalInput;
            rigidbody.MovePosition(rigidbody.position + movementDirection * topSpeed * Time.deltaTime);

            // Handle jump input
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                TryJump();
            }
        
            SetStateAndAnimation(verticalInput);
        }

        private void SetStateAndAnimation(float moveVertical)
        {
            if (!isGrounded)
            {
                protagonistState = ProtagonistState.Jumping;
            }
            else if (Mathf.Abs(moveVertical) > 0.1f)
            {
                protagonistState = ProtagonistState.Walking;
            }
            else
            {
                protagonistState = ProtagonistState.Idle;
            }

            animator.SetFloat("Speed", Mathf.Abs(moveVertical));
            // animator.SetBool("IsJumping", !isGrounded);
        }

        private void TryJump()
        {
            animator.SetTrigger("Jump");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                animator.ResetTrigger("Jump");
            }
        }
    }
}