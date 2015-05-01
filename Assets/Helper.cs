using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// A class consisting of a bunch of helper functions.
public class Helper
{
    // A static instance of the Random class to functions based on probability.
    public static Random randomModule = new Random();

    // Return a random float between <minVal> and <maxVal>.
    public static float NextFloat(float minVal, float maxVal)
    {
        var result = (randomModule.NextDouble() * (maxVal - (double)minVal)) + minVal;
        return (float)result;
    }

}
