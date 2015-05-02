using System;
using System.Collections.Generic;
using UnityEngine;

// A class that manages the match.
public class MatchManager : MonoBehaviour
{

    const float TIME_DISPLAY_RULES = 5f; // Amount of time spent to display the rules
    float timerDisplayRules; // Timer to keep track of how long to display the rules
    private bool displayRules; // Indicates whether we should display the rules

    GUIStyle startMessageStyle; // The style used for the text displaying the "DRAW" message.
    const float START_MESSAGE_WIDTH = 100f; // The width of the draw message.
    const float START_MESSAGE_HEIGHT = 50f; // The height of the draw message.

    float timerToWaitAfterDrawBeforeBattle; // Amount of time to wait, after the message "DRAW" has appeared, and before the battle starts.

    private bool displayGoMessage; // Indicates whether we should display the "GO!" message. 
    private float timerToDisplayGoMessage; // Measures how the countdown over how long we should wait before displaying the "GO!" message.
    private const float TIME_TO_DISPLAY_GO_MESSAGE = .5f; // Amount of time to display the "GO!" message.

    public List<GameObject> playerList; // A list of GameObjects representing both players.

    // Exclusive to Unity testing environments only. If run as the executable itself and not in the Unity window,
    // this assignment will be ignored. This variable manages the mapping of controls.
    #if UNITY_EDITOR
        ControlManager CONTROL_MANAGER;
    #endif

    // Load the match screen.
    void Start()
    {

        // Set all the timers for doing stuff on-screen.
        timerToWaitAfterDrawBeforeBattle = Helper.NextFloat(5f, 10f);
        timerToDisplayGoMessage = TIME_TO_DISPLAY_GO_MESSAGE;
        timerDisplayRules = TIME_DISPLAY_RULES;

        // Don't display the messages for "GO!" or "DRAW!" at all, until time warrants it. 
        displayGoMessage = false;
        displayRules = false;

        // Initialize the style for displaying the "GO!" and "DRAW!" messages.
        startMessageStyle = new GUIStyle();
        startMessageStyle.normal.textColor = Color.black;
        startMessageStyle.font = Global.STENCIL;
        startMessageStyle.fontSize = 200;
        startMessageStyle.alignment = TextAnchor.MiddleCenter;

        // Retrieve the list of all player character objects, and add the script component
        // PlayerScript.cs to each player.
        playerList = new List<GameObject>() { GameObject.Find("Player1"), GameObject.Find("Player2") };

        // If the namespace UnityEditor is defined, update the values of the input axes if the flag is set to true (
        // do this in Global.cs).
        #if UNITY_EDITOR
            if (Global.REDEFINE_INPUT_AXES) {
                CONTROL_MANAGER = new ControlManager();
                CONTROL_MANAGER.RedefineInputManager();
            }
        #endif

        // Get a list of all the consoles and controls plugged in.
        string[] joystickList = Input.GetJoystickNames();
        int numJoysticks = joystickList.Length;
        int numJoysticksAssigned = Math.Min(2, numJoysticks);

        for (int i = 0; i < numJoysticksAssigned; i++) {
            // Add the player script if it has not been added, and start it.
            if (playerList[i].GetComponent<PlayerScript>() == null) {
                playerList[i].AddComponent<PlayerScript>();
            }
            playerList[i].GetComponent<PlayerScript>().PreStart(i + 1);
        }

    }

    // Update the match every frame.
    void Update()
    {

        // Display the rules on-screen.
        if (timerDisplayRules > 0f) {
            displayRules = true;
            timerDisplayRules -= Time.deltaTime;
            startMessageStyle.fontSize = 150;
        }
        else {
            displayRules = false;
            startMessageStyle.fontSize = 200;
        }

        // Then, wait a random amount of time between 5 to 10 seconds.
        if (timerDisplayRules <= 0f && timerToWaitAfterDrawBeforeBattle > 0f) {
            timerToWaitAfterDrawBeforeBattle -= Time.deltaTime;
        }

        // Then, display the message to begin shooting.
        if (timerToWaitAfterDrawBeforeBattle <= 0f && timerToDisplayGoMessage > 0f) {
            timerToDisplayGoMessage -= Time.deltaTime;
            displayGoMessage = true;
        }
        else {
            displayGoMessage = false;
        }

    }

    // Update the display on-screen every frame.
    void OnGUI()
    {
        // Display the rules for the first 5 seconds (placeholder display for now).
        if (displayRules) {

            GUI.Label(new Rect((float)Screen.width / 2f - (START_MESSAGE_WIDTH / 2f),
                               (float)Screen.height / 2f - (START_MESSAGE_HEIGHT / 2f),
                               START_MESSAGE_WIDTH,
                               START_MESSAGE_HEIGHT), "VOXEL SUCKS ASS!", startMessageStyle);
        }

        // Display the message to begin shooting after somewhere between 5 to 10 seconds.
        if (displayGoMessage) {
            GUI.Label(new Rect((float)Screen.width / 2f - (START_MESSAGE_WIDTH / 2f),
                       (float)Screen.height / 2f - (START_MESSAGE_HEIGHT / 2f),
                       START_MESSAGE_WIDTH,
                       START_MESSAGE_HEIGHT), "DRAW!", startMessageStyle);
        }

    }

}
