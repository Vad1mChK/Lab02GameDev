using TMPro;
using UnityEngine;

namespace Text
{
    public class TextDataLoader : MonoBehaviour
    {
        [SerializeField] private TextData textData;
        [SerializeField] private TMP_Text textElement;
        
        void Start()
        {
            if (textElement == null)
            {
                textElement = GetComponent<TMP_Text>();
            }
            textElement.text = textData.textContent;
        }
    }
}
