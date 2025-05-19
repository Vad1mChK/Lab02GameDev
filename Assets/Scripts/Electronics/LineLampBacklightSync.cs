using UnityEngine;

namespace Electronics {
    public class LineLampBacklightSync : MonoBehaviour
    {
        [SerializeField] private Light targetLight;
        [SerializeField] private GameObject targetBacklightPlane;
        
        private MaterialPropertyBlock _propBlock;
        private Renderer _backlightRenderer;

        void Start()
        {
            if (!targetLight || !targetBacklightPlane) return;
            
            _propBlock = new MaterialPropertyBlock();
            _backlightRenderer = targetBacklightPlane.GetComponent<Renderer>();
            
            // Initial sync
            UpdateColors();
        }

        // void Update()
        // {
        //     if (!targetLight || !targetBacklightPlane) return;
        //     
        //     UpdateColors();
        // }

        private void UpdateColors()
        {
            // Get current light color with intensity
            var lightColor = Color.Lerp(targetLight.color, Color.white, 0.5f);
            
            // Apply to both light and material
            _backlightRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", lightColor); // Correct property name for Unlit/Color
            _backlightRenderer.SetPropertyBlock(_propBlock);
        }

        public void SetColor(Color newColor)
        {
            if (!targetLight) return;
            
            targetLight.color = newColor;
            UpdateColors();
        }
    }
}