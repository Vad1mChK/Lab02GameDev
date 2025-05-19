using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static float Sigmoid(float x) =>
            1f / (1f + Mathf.Exp(-x));
    }
}