using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// A namespace (and by extension, a class) containing several extension methods.
namespace ExtendSpace
{
    public static class Extend
    {
        // Return true if the given <obj> is in the list of given <items>, and false otherwise.
        public static bool IsAny<T>(this T obj, params T[] items)
        {
            return items.ToList().Contains(obj);
        }

        // Return true if the given control is a button, and false otherwise.
        public static bool IsButton(this PS4Control psc)
        {
            return psc.IsAny(PS4Control.SQUARE, PS4Control.TRIANGLE, PS4Control.X, PS4Control.CIRCLE, PS4Control.L1,
                PS4Control.L2, PS4Control.R1, PS4Control.R2);
        }

        // Return true if the given control is a stick, and false otherwise.
        public static bool IsStick(this PS4Control psc)
        {
            return psc.IsAny(PS4Control.VERTICAL_LEFT_STICK, PS4Control.VERTICAL_RIGHT_STICK,
                             PS4Control.HORIZONTAL_LEFT_STICK, PS4Control.HORIZONTAL_RIGHT_STICK);
        }

    }

}
