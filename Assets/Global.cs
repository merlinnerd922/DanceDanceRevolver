using UnityEngine;
using UnityEngine.UI;
using UnityColor = UnityEngine.Color;

// Class consisting of several global variables.
public static class Global
{
    public static Font STENCIL = Resources.Load<Font>("STENCIL"); // Stencil font-face
    public static bool REDEFINE_INPUT_AXES = true; // Indicate whether we should update the input manager.
    public static bool TEST_MODE = false;
    public static UnityColor GOLDENROD = new UnityColor(218f / 255f, 165f / 255f, 32f / 255f);

    public static int NUM_PLAYERS = 2;
}
