using ExtendSpace;
using System;
using UnityEngine;

// A class denoting a script attached to each player object.
public class PlayerScript : MonoBehaviour
{
    int playerNum; // Player's player number.
    string stringRepOnScreen; // On-screen representation of the player's control pressed.
    private GUIStyle controlDisplayStyle; // The font style of the display of the controls pressed.

    // Call this special method to load characteristics associated with this player.
    public void PreStart(int _playerNum)
    {
        playerNum = _playerNum; // Player number

        // Create style for displaying info on the control most recently pressed.
        controlDisplayStyle = new GUIStyle();
        controlDisplayStyle.normal.textColor = Color.black;
        controlDisplayStyle.font = Global.STENCIL;
        controlDisplayStyle.fontSize = 50;
    }

    // Update this character based on controls pressed..
    void UpdateControls()
    {
        float inputVal, inputValAbs;
        bool inputBoolVal;

        // Check each given possible control button/axis to see if it's been pressed/toggled.
        foreach (PS4Control c in Enum.GetValues(typeof(PS4Control))) {

            // For buttons, check to see if they've been pressed, and if so, update the most recently pressed control.
            if (c.IsButton()) {
                inputBoolVal = Input.GetButton(Helper.GetPlayerAxis(playerNum, c));
                if (inputBoolVal) {
                    stringRepOnScreen = c.ToString();
                }
            }


            else {
                inputVal = Input.GetAxis(Helper.GetPlayerAxis(playerNum, c));
                inputValAbs = Math.Abs(inputVal);

                // If the amount the control has been toggled is above the threshold:
                if (inputValAbs > 0.05f) {

                    // Display the info if the control manipulated was NOT a stick.
                    if (!c.IsStick()) {
                        stringRepOnScreen = c.ToString();
                    }

                    // Otherwise, check if the left stick has been toggled, and if so, update the most recently pressed control to 
                    // indicate the left stick has been pressed.
                    else if (inputValAbs > 0.5f) {

                        // If the left stick has been moved vertically (more than it has horizontally):
                        if (c == PS4Control.VERTICAL_LEFT_STICK) {
                            if (inputValAbs > Math.Abs(Input.GetAxis(Helper.GetPlayerAxis(playerNum, PS4Control.HORIZONTAL_LEFT_STICK)))) {
                                stringRepOnScreen = c.ToString();
                                break;
                            }
                        }

                        // If the left stick has been moved horizontally (more than it has vertically):
                        else if (c == PS4Control.HORIZONTAL_LEFT_STICK) {
                            if (inputValAbs > Math.Abs(Input.GetAxis(Helper.GetPlayerAxis(playerNum, PS4Control.VERTICAL_LEFT_STICK)))) {
                                stringRepOnScreen = c.ToString();
                                break;
                            }
                        }

                    }
                }

            }
        }
    }

    // Update the player control.
    void Update()
    {
        // Update the player to reflect pressed input controls.
        UpdateControls();
    }

    // On every frame, display the control that was recently pressed.
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 50, 50), stringRepOnScreen, controlDisplayStyle);
    }

}
