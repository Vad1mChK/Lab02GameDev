using System;
using UnityEngine;

namespace Text
{
    [CreateAssetMenu(fileName = "Text Data", menuName = "Text/Text Data")]
    [Serializable]
    public class TextData : ScriptableObject
    {
        [Multiline(16)] public string textContent;
    }
}
