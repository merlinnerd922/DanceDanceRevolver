using System.Collections.Generic;
using System.Linq;

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
                PS4Control.L2, PS4Control.R1, PS4Control.R2, PS4Control.R3, PS4Control.L3);
        }

        // Return true if the given control is a button, and false otherwise.
        public static bool IsButton(this PS4Pressable psc)
        {
            return psc.IsAny(PS4Pressable.SQUARE, PS4Pressable.TRIANGLE, PS4Pressable.X, PS4Pressable.CIRCLE, PS4Pressable.L1,
                PS4Pressable.L2, PS4Pressable.R1, PS4Pressable.R2, PS4Pressable.L3, PS4Pressable.R3);
        }

        // Return true if the given control is a stick, and false otherwise.
        public static bool IsStick(this PS4Control psc)
        {
            return psc.IsAny(PS4Control.VERTICAL_LEFT_STICK, PS4Control.VERTICAL_RIGHT_STICK,
                             PS4Control.HORIZONTAL_LEFT_STICK, PS4Control.HORIZONTAL_RIGHT_STICK);
        }


        // Given a list interface of items, return a random item from the List.
        public static T GetRandomItem<T>(this IList<T> lst)
        {
            return lst[Helper.Next(0, lst.Count())];
        }


        public static List<T> GetNRandomItems<T>(this List<T> itemLists, int numItems)
        {
            List<T> returnLst = new List<T>() { };

            for (int i = 0; i < numItems; i++) {
                returnLst.Add(itemLists.GetRandomItem());
            }

            return returnLst;
        }

        public static string ToOnScreenRep(this List<PS4Pressable> listPressables, int numSymbolsPressed)
        {
            int numSymbolsPressedTemp = numSymbolsPressed; 

            string returnStr = "";
            foreach (PS4Pressable p in listPressables) {
                if (numSymbolsPressedTemp <= 0) {
                    returnStr += Player.pressableCharacterSymbolMapping[p] + " ";
                }
                else {
                    numSymbolsPressedTemp--;
                }
            }
            return returnStr;
        }

        public static PS4Pressable ToPS4Pressable(this PS4Control c)
        {
            return Helper.controlPressableMapping[c];
        }



    }

}
