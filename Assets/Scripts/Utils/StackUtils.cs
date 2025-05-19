using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class StackUtils
    {
        public static string PrettyPrint<T>(this Stack<T> stack) where T : Enum
        {
            if (stack == null || stack.Count == 0)
                return "Empty Stack";
        
            // Reverse to show bottom-to-top order (natural stack order is top-to-bottom)
            return string.Join(" -> ", stack.Reverse().Select(e => e.ToString()));
        }
    }
}