using UnityEngine;

public class ProtagonistController : MonoBehaviour
{
    [Header("Controls")]
    [Tooltip("Key to be pressed for jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Movement")]
    [SerializeField] private float topSpeed = 5f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Misc")]
    [SerializeField] private Rigidbody rigidbody;

    private int groundCount = 0;
    
    void Start()
    {
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
        if (Input.GetKeyDown(jumpKey) && groundCount > 0)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ++groundCount;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            --groundCount;
        }
    }
}