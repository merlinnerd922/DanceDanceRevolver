using UnityEngine;

// Class consisting of several global variables.
public static class Global
{
    public static Font STENCIL = Resources.Load<Font>("STENCIL"); // Stencil font-face
    public static bool REDEFINE_INPUT_AXES = true; // Indicate whether we should update the input manager.
}
