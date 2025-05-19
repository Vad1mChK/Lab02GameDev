using UnityEngine;

public class PanoramaCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private bool autoRotate = true;
    [SerializeField] private float autoRotateSpeed = 0.1f;

    private Vector3 lastMousePosition;

    void Update()
    {
        // Auto-rotation
        if (autoRotate)
        {
            transform.Rotate(Vector3.up * autoRotateSpeed * Time.deltaTime);
        }

        // Manual rotation
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Rotate(Vector3.up * -delta.x * rotationSpeed * Time.deltaTime);
            lastMousePosition = Input.mousePosition;
        }
    }
}