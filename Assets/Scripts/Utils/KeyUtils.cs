using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class KeyUtils
    {
        public static List<KeyCode> ListOfKeysDown()
        {
            return Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>()
                .Where(Input.GetKeyDown)
                .ToList();
        }
        
        public static bool AnyKeyDownFromSubset(this IEnumerable<KeyCode> keys)
        {
            foreach (KeyCode key in keys)
            {
                if (Input.GetKeyDown(key)) return true;
            }
            return false;
        }

        // Range generators
        public static IEnumerable<KeyCode> Letters(bool includeLowerCase = false)
        {
            // Upper case A-Z (KeyCode.A to KeyCode.Z)
            for (KeyCode k = KeyCode.A; k <= KeyCode.Z; k++)
            {
                yield return k;
            }

            // Lower case a-z (KeyCode doesn't differentiate case)
            if (includeLowerCase)
            {
                // Note: KeyCode values are same for upper/lower case
                // This is just for API clarity
                for (KeyCode k = KeyCode.A; k <= KeyCode.Z; k++)
                {
                    yield return k;
                }
            }
        }

        public static IEnumerable<KeyCode> Numbers(bool includeKeypad = true)
        {
            // Top row numbers (0-9)
            for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
            {
                yield return k;
            }

            // Keypad numbers
            if (includeKeypad)
            {
                for (KeyCode k = KeyCode.Keypad0; k <= KeyCode.Keypad9; k++)
                {
                    yield return k;
                }
            }
        }

        public static IEnumerable<KeyCode> CustomRange(KeyCode start, KeyCode end)
        {
            int startVal = (int)start;
            int endVal = (int)end;
        
            for (int i = startVal; i <= endVal; i++)
            {
                yield return (KeyCode)i;
            }
        }
    }
}