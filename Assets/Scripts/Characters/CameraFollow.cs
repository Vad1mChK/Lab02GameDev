using UnityEngine;

namespace Characters
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private GameObject cameraPivot;
        [SerializeField] private Vector3 offsetFromPivot = new Vector3(0, 0, -2);
        [SerializeField] private float maxAngleAmplitudeDegrees = 80f;
        [SerializeField] private float cameraRotationSpeed = 150f;
    
        private Quaternion _baseRotation;
        private float _currentVerticalAngle;
        private const float MaxSigmoidBoundary = 5f;

        void Start()
        {
            _baseRotation = cameraPivot.transform.rotation;
            _currentVerticalAngle = 0f;
        }

        void Update()
        {
            HandleCameraRotation();
            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        private void HandleCameraRotation()
        {
            float mouseY = Input.GetAxis("Mouse Y");
        
            // Apply sigmoid-like smoothing to the rotation input
            float smoothedInput = Mathf.Clamp(mouseY * cameraRotationSpeed * Time.deltaTime, 
                -MaxSigmoidBoundary, 
                MaxSigmoidBoundary);
        
            _currentVerticalAngle += smoothedInput;
        
            // Clamp vertical angle using sigmoid-inspired limits
            float maxRadians = maxAngleAmplitudeDegrees * Mathf.Deg2Rad;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, -maxRadians, maxRadians);
        }

        private void UpdateCameraPosition()
        {
            transform.position = cameraPivot.transform.position + 
                                 cameraPivot.transform.rotation * offsetFromPivot;
        }

        private void UpdateCameraRotation()
        {
            // Combine base rotation with vertical offset
            Quaternion verticalRotation = Quaternion.AngleAxis(-_currentVerticalAngle * Mathf.Rad2Deg, 
                Vector3.right);
            transform.rotation = cameraPivot.transform.rotation * verticalRotation;
        }
    }
}