using System;
using System.Collections.Generic;
using System.Linq;
using ExtendSpace;

// A class consisting of a bunch of helper functions.
public class Helper
{
    // A static instance of the Random class to functions based on probability.
    public static Random randomModule = new Random();

    public static List<PS4Control> PS4Controls = Enum.GetValues(typeof(PS4Control)).Cast<PS4Control>().ToList();

    public static List<PS4Pressable> PS4Pressables = Enum.GetValues(typeof(PS4Pressable)).Cast<PS4Pressable>().ToList();

    public static List<PS4Control> PS4Buttons = PS4Controls.Where(x => x.IsButton()).ToList();

    public static List<PS4Pressable> PS4AxesPressables = PS4Pressables.Where(x => !x.IsButton()).ToList();

    public static List<PS4Pressable> PS4AxesSequencePressables = PS4Pressables.Where(x => !x.IsButton() && !x.IsAny(PS4Pressable.L3, PS4Pressable.R3)).ToList();

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


    public static List<PS4Pressable> GetRandomListOfKeysToPress()
    {
        return Helper.PS4Pressables.Where(x => x.IsButton() && !x.IsAny(PS4Pressable.R3, PS4Pressable.L3)).ToList().GetNRandomItems(Helper.Next(3, 6));
    }

    // Return a random value between minValue and MaxValue.
    public static int Next(int minValue, int maxValue)
    {
        return randomModule.Next(minValue, maxValue);
    }

}
