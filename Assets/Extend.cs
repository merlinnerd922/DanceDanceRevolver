using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendSpace
{
    public static class Extend
    {
        public static bool IsAny<T>(this T obj, params T[] items)
        {
            return items.ToList().Contains(obj);
        }

        public static bool IsButton(this PS4Control psc)
        {
            return psc.IsAny(PS4Control.SQUARE, PS4Control.TRIANGLE, PS4Control.X, PS4Control.CIRCLE, PS4Control.L1,
                PS4Control.L2, PS4Control.R1, PS4Control.R2);
        }

        public static bool IsStick(this PS4Control psc)
        {
            return psc.IsAny(PS4Control.VERTICAL_LEFT_STICK, PS4Control.VERTICAL_RIGHT_STICK,
                             PS4Control.HORIZONTAL_LEFT_STICK, PS4Control.HORIZONTAL_RIGHT_STICK);
        }

    }

}
