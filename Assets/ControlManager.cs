#if UNITY_EDITOR
using ExtendSpace;
using System;
using System.Collections.Generic;
using UnityEditor;

// A class that manages the input and controls of the game.
class ControlManager
{
    public Dictionary<PS4Control, int?> controlAxisMapping; // A mapping of PS4 controls to certain axes.
    public Dictionary<PS4Control, int?> controlButtonMapping; // A mapping of PS4 controls to certain buttons.
    
    public ControlManager()
    {
        // Initialize mappings of PS4 controls to axes and buttons.
        controlAxisMapping = new Dictionary<PS4Control, int?>() { 
            { PS4Control.VERTICAL, 8 }, // CORRECT
            { PS4Control.HORIZONTAL, 7}, // CORRECT
            { PS4Control.L2, 5}, 
            { PS4Control.R2, 6},
            { PS4Control.HORIZONTAL_LEFT_STICK, 1}, // CORRECT
            { PS4Control.VERTICAL_LEFT_STICK, 2}, // CORRECT
            { PS4Control.HORIZONTAL_RIGHT_STICK, 3}, // CORRECT
            { PS4Control.VERTICAL_RIGHT_STICK, 4}
        };

        controlButtonMapping = new Dictionary<PS4Control, int?>() { 
            { PS4Control.SQUARE, 0 },
            { PS4Control.X, 1},
            { PS4Control.CIRCLE, 2},
            { PS4Control.TRIANGLE, 3},
            { PS4Control.L1, 4},
            { PS4Control.R1, 5},
            { PS4Control.L2, 6},
            { PS4Control.R2, 7},
            { PS4Control.VERTICAL, 13},
            { PS4Control.HORIZONTAL, 13}
        };
    }

    // Given a serialized property and the name of one of the property's descendants, return the descendant
    // with that name.
    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do {
            if (child.name == name)
                return child;
        }
        while (child.Next(false));
        return null;
    }

    // Given the name of an axis, return true if it is defined and false otherwise.
    private static bool AxisDefined(string axisName)
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.Next(true);
        axesProperty.Next(true);
        while (axesProperty.Next(false)) {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);
            if (axis.stringValue == axisName)
                return true;
        }
        return false;
    }

    // Add the given axis to the list of axes.
    private static void AddAxis(InputAxis axis)
    {
        if (AxisDefined(axis.name))
            return;

        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.arraySize++;
        serializedObject.ApplyModifiedProperties();

        SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
        GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

        serializedObject.ApplyModifiedProperties();

    }
    // Reset the game's input axes to the given values.
    public void RedefineInputManager()
    {
        int controlAxis, controlButton, controlGravity;
        string joystickButtonText;
        float controlSensitivity;

        // Clear out the set of input definitions.
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
        axesProperty.ClearArray();
        serializedObject.ApplyModifiedProperties();

        // For each player, define all of the different input axes.
        for (int j = 0; j < 1; j++) {
            foreach (PS4Control psc in Enum.GetValues(typeof(PS4Control))) {

                controlAxis = controlAxisMapping.ContainsKey(psc) ? (int)controlAxisMapping[psc] : 1;
                controlButton = controlButtonMapping.ContainsKey(psc) ? (int)controlButtonMapping[psc] : -1;

                joystickButtonText = controlButton != -1 ? "joystick button " + controlButton : "";
                controlGravity = psc.IsButton() ? 1 : 1000;
                controlSensitivity = psc.IsStick() ? 1f : 1000f;

                AddAxis(new InputAxis() {
                    name = Helper.GetPlayerAxis(j + 1, psc),
                    dead = 0.001f,
                    type = AxisType.JoystickAxis,
                    axis = controlAxis,
                    joyNum = (j + 1),
                    positiveButton = joystickButtonText,
                    gravity = controlGravity,
                    sensitivity = controlSensitivity
                });
            }
        }

        // Save all info on the new input axes to the input manager
        AssetDatabase.SaveAssets(); 

    }

}
#endif
