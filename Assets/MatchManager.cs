using System.Collections.Generic;
using UnityEngine;

// A class that manages the match.
public class MatchManager : MonoBehaviour {

    const float TIME_DISPLAY_RULES = 5f; // Amount of time spent to display the rules
    float timerDisplayRules; // Timer to keep track of how long to display the rules

    const float TIME_BEFORE_DRAWING_GUNS = .5f; // Time to wait (a small period) before the word "DRAW!" is displayed on-screen.
    float timerBeforeDrawingGuns; // Timer that keeps track of when to display the word "DRAW!" on-screen.

    const float TIME_TO_DISPLAY_DRAW_MESSAGE = .5f; // Amount of time the message "draw" message should display.
    float timerToDisplayDrawMessage; // Timer to keep track of displaying the draw message.
    bool displayDrawMessage; // Indicates whether we should display the "DRAW" message on-screen.

    GUIStyle startMessageStyle; // The style used for the text displaying the "DRAW" message.
    const float DRAW_MESSAGE_WIDTH = 100f; // The width of the draw message.
    const float DRAW_MESSAGE_HEIGHT = 50f; // The height of the draw message.

    float timerToWaitAfterDrawBeforeBattle; // Amount of time to wait, after the message "DRAW" has appeared, and before the battle starts.

    private bool displayGoMessage; // Indicates whether we should display the "GO!" message. 
    private float timerToDisplayGoMessage; // Measures how the countdown over how long we should wait before displaying the "GO!" message.
    private const float TIME_TO_DISPLAY_GO_MESSAGE = .5f; // Amount of time to display the "GO!" message.

    public List<GameObject> playerList;

	// Load the match screen.
	void Start () {

        // Set all the timers for doing stuff on-screen.
        timerBeforeDrawingGuns = TIME_BEFORE_DRAWING_GUNS;
        timerToDisplayDrawMessage = TIME_TO_DISPLAY_DRAW_MESSAGE;
        timerToWaitAfterDrawBeforeBattle = Helper.NextFloat(5f, 10f);
        timerToDisplayGoMessage = TIME_TO_DISPLAY_GO_MESSAGE;

        // Don't display the messages for "GO!" or "DRAW!" at all, until time warrants it. 
        displayDrawMessage = false;
        displayGoMessage = false;

        // Initialize the style for displaying the "GO!" and "DRAW!" messages.
        startMessageStyle = new GUIStyle();
        startMessageStyle.normal.textColor = Color.black;
        startMessageStyle.font = Global.STENCIL;
        startMessageStyle.fontSize = 200;
        startMessageStyle.alignment = TextAnchor.MiddleCenter;
        
        // Retrieve the list of all player character objects, and add the script component
        // PlayerScript.cs to each player.
        playerList = new List<GameObject>() {  GameObject.Find("Player1"), GameObject.Find("Player2") };
        foreach (GameObject player in playerList) {

            if (player.GetComponent<PlayerScript>() == null) {
                player.AddComponent<PlayerScript>();
            }
            player.GetComponent<PlayerScript>().PreStart();
        }


	}
	
	// Update the match every frame.
	void Update () {

        // Update the timer before displaying the words DRAW! on-screen.
        if (timerBeforeDrawingGuns > 0f) {
            timerBeforeDrawingGuns -= Time.deltaTime;
        }

        // Display the words DRAW! on-screen if we are counting down the timer; otherwise, DON'T display the draw message.
        if (timerBeforeDrawingGuns <= 0f && timerToDisplayDrawMessage > 0f) {
            timerToDisplayDrawMessage -= Time.deltaTime;
            displayDrawMessage = true;
        }
        else {
            displayDrawMessage = false;
        }

        // Then, wait a random amount of time between 5 to 10 seconds.
        if (timerToDisplayDrawMessage <= 0f && timerToWaitAfterDrawBeforeBattle > 0f) {
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
        // Display the draw message when the flag for it is activated.
        if (displayDrawMessage) {
            GUI.Label(new Rect((float)Screen.width / 2f - (DRAW_MESSAGE_WIDTH / 2f), 
                               (float)Screen.height / 2f - (DRAW_MESSAGE_HEIGHT / 2f), 
                               DRAW_MESSAGE_WIDTH, 
                               DRAW_MESSAGE_HEIGHT), "DRAW!", startMessageStyle);
        }

        // Display the message to begin shooting after somewhere between 5 to 10 seconds.
        if (displayGoMessage) {
            GUI.Label(new Rect((float)Screen.width / 2f - (DRAW_MESSAGE_WIDTH / 2f),
                       (float)Screen.height / 2f - (DRAW_MESSAGE_HEIGHT / 2f),
                       DRAW_MESSAGE_WIDTH,
                       DRAW_MESSAGE_HEIGHT), "SHOOT!", startMessageStyle);
        }



    }

}
