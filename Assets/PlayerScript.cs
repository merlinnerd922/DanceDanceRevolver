using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ExtendSpace;

// A class denoting a script attached to each player object.
public class PlayerScript : MonoBehaviour
{
    int playerNum; // Player's player number.
    string stringRepOnScreen; // On-screen representation of the player's control pressed.
    private GUIStyle controlDisplayStyle;

    // Call this special method to load characteristics associated with this player.
    public void PreStart(int _playerNum)
    {
        playerNum = _playerNum;

        controlDisplayStyle = new GUIStyle();
        controlDisplayStyle.normal.textColor = Color.black;
        controlDisplayStyle.font = Global.STENCIL;
        controlDisplayStyle.fontSize = 50;
        //controlDisplayStyle.alignment = TextAnchor.MiddleCenter;
    }

    // Update the player control.
    void Update()
    {
        float inputVal;
        bool inputBoolVal;
        
        foreach (PS4Control c in Enum.GetValues(typeof(PS4Control))) {

            if (c.IsButton()) {
                inputBoolVal = Input.GetButton(Helper.GetPlayerAxis(playerNum, c));
                if (inputBoolVal) {
                    stringRepOnScreen = c.ToString();
                }
            }

            else {
                inputVal = Input.GetAxis(Helper.GetPlayerAxis(playerNum, c));
                if (Math.Abs(inputVal) > 0.05f) {
                    if (!c.IsStick()) {
                        stringRepOnScreen = c.ToString();
                    }


                    if (c == PS4Control.HORIZONTAL_LEFT_STICK) {
                        stringRepOnScreen = c.ToString();
                    }

                    //else if (c.IsAny(PS4Control.HORIZONTAL_LEFT_STICK, PS4Control.HORIZONTAL_RIGHT_STICK)) {
                    //    stringRepOnScreen = c.ToString();
                    //}
                    //else if (Math.Abs(inputVal) > 0.5f) {
                    //    if (c == PS4Control.HORIZONTAL_LEFT_STICK) {
                    //        stringRepOnScreen = c.ToString();
                    //    }

                    //    else if (c == PS4Control.HORIZONTAL_RIGHT_STICK) {
                    //        stringRepOnScreen = c.ToString();
                    //    }

                    //    else if (c == PS4Control.VERTICAL_LEFT_STICK) {
                    //        stringRepOnScreen = c.ToString();
                    //    }

                    //    else if (c == PS4Control.VERTICAL_RIGHT_STICK) {
                    //        stringRepOnScreen = c.ToString();
                    //    }
                    //}



                    

                }
            }


            

        //    x = Input.GetAxis(Helper.GetPlayerAxis(playerNum, c));

        //    if (c.IsAny(PS4Control.L2, PS4Control.R2)) 
        //    {
        //        if (Math.Abs(x) > 0.05f && x != -1f) 
        //        {
        //            stringRepOnScreen = c.ToString();
        //        }
        //    }

        //    else if (Math.Abs(x) > 0.05f) {
        //        stringRepOnScreen = c.ToString();
        //    }

        }


       
         
        


    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 50, 50), stringRepOnScreen, controlDisplayStyle);
    }

}
