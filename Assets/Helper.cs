using ExtendSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysRandom = System.Random;
using UnityColor = UnityEngine.Color;

// A class consisting of a bunch of helper functions.
public class Helper
{

    // A static instance of the Random class to functions based on probability.
    public static SysRandom randomModule = new SysRandom();

    // Denotes the list of all controls on a PS4.
    public static List<PS4Control> PS4Controls = Enum.GetValues(typeof(PS4Control)).Cast<PS4Control>().ToList();

    // Denotes the list of all controls that represent buttons on a PS4.
    public static List<PS4Control> PS4Buttons = PS4Controls.Where(x => x.IsButton()).ToList();

    // Denotes a list of all "pressables" (possible inputs that can be given to the machine).
    public static List<PS4Pressable> PS4Pressables = Enum.GetValues(typeof(PS4Pressable)).Cast<PS4Pressable>().ToList();

    // Denotes a list of all "pressables" that denote axes.
    public static List<PS4Pressable> PS4AxesPressables = PS4Pressables.Where(x => !x.IsButton()).ToList();

    // A mapping of PS4 controls to the pressable inputs (meant to map cases where there is a Pressable with the same 
    // naem as a PS4 control).
    public static Dictionary<PS4Control, PS4Pressable> controlPressableMapping = PS4Buttons.ToDictionary(i => i, i => (PS4Pressable)(Enum.Parse(typeof(PS4Pressable), i.ToString())));

    // Return a random float between <minVal> and <maxVal>.
    public static float NextFloat(float minVal, float maxVal)
    {
        var result = (randomModule.NextDouble() * (maxVal - (double)minVal)) + minVal;
        return (float)result;
    }

    // Given a player number and a control, return the axis (as a string) associated with them.
    public static string GetPlayerAxis(int playerNum, PS4Control c)
    {
        return "Player" + playerNum.ToString() + c;
    }

    // Return a list of 3 - 5 pressable icons to be pressed by a player.
    public static List<PS4Pressable> GetRandomListOfKeysToPress()
    {
        return Helper.PS4Pressables.Where(x => x.IsButton() && !x.IsAny(PS4Pressable.R3, PS4Pressable.L3)).ToList().GetNRandomItems(Helper.Next(3, 6));
    }

    // Return a random value between minValue and MaxValue.
    public static int Next(int minValue, int maxValue)
    {
        return randomModule.Next(minValue, maxValue);
    }

    // Given a Vector3 representing angles, return a normalized version of the angles.
    public static Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    // Given a float representing an angle, normalize it by changing it to an equivalent representation between 0 and 360 degrees.
    static float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;
        while (angle < 0)
            angle += 360;
        return angle;
    }

    // Return a random item given the parameter items.
    internal static T GetRandom<T>(params T[] random)
    {
        return random.GetRandomItem<T>();
    }
}
