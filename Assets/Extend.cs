using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public static Rect AddToX(this Rect r, float addValToX) {
            Rect returnRect = new Rect(r.xMin, r.yMin, r.width, r.height);
            returnRect.x += addValToX;
            return returnRect;
        }

        public static Vector3 ToVector3(this Quaternion q1)
        {
            float sqw = q1.w * q1.w;
            float sqx = q1.x * q1.x;
            float sqy = q1.y * q1.y;
            float sqz = q1.z * q1.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.x * q1.w - q1.y * q1.z;
            Vector3 v;

            if (test > 0.4995f * unit) { // singularity at north pole
                v.y = 2f * Mathf.Atan2(q1.y, q1.x);
                v.x = Mathf.PI / 2;
                v.z = 0;
                return Helper.NormalizeAngles(v * Mathf.Rad2Deg);
            }
            if (test < -0.4995f * unit) { // singularity at south pole
                v.y = -2f * Mathf.Atan2(q1.y, q1.x);
                v.x = -Mathf.PI / 2;
                v.z = 0;
                return Helper.NormalizeAngles(v * Mathf.Rad2Deg);
            }
            Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
            v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
            return Helper.NormalizeAngles(v * Mathf.Rad2Deg);
        }

    }

}
