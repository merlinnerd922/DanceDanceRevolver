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
    private static float timerToDisplayGoMessage; // Measures how the countdown over how long we should wait before displaying the "GO!" message.
    private const float TIME_TO_DISPLAY_GO_MESSAGE = .5f; // Amount of time to display the "GO!" message.

    public List<GameObject> playerList; // A list of GameObjects representing both players.
    public List<Player> playerScripts; // A list corresponding to the scripts of the player GameObjects.

    // Indicates whether the match has begun (we determine this by determining whether we have displayed
    // the message to begin shooting for the appropriate amount of time).
    public static bool matchHasBegun
    {
        get
        {
            return timerToDisplayGoMessage <= 0f;
        }
    }

    // Exclusive to Unity testing environments only. If run as the executable itself and not in the Unity window,
    // this assignment will be ignored. This variable manages the mapping of controls.
    #if UNITY_EDITOR
        ControlManager CONTROL_MANAGER;
    #endif

    public static Camera mainCamera; // A game object representing the main camera.
    private GameVictoryStatus gameVictoryStatus; // An object describing how the game ended: a victory for one player, or a tie?
    private string finalVictoryMessage;
    private GUIStyle gameEndsMessageStyle;

    public bool gameOver
    {
        get
        {
            return playerScripts[0].currentHP <= 0 || playerScripts[1].currentHP <= 0;
        }

    }

    public const float TIME_TO_WAIT_BEFORE_RESTART = 5f;
    public float timerToWaitBeforeRestart;

    private Rect levelReloadingRectangle = new Rect(Screen.width / 2f - 100f / 2f,
                                                    (0.75f * Screen.height) - 50f / 2f,
                                                    100f, 50f);
    private string levelReloadingMessage = "LOADING ANOTHER MATCH, PLEASE WAIT...";
    private GUIStyle levelReloadingStyle;

    // Load the match screen.
    void Start()
    {



        // Set all the timers for doing stuff on-screen.
        timerToWaitAfterDrawBeforeBattle = Global.TEST_MODE ? 0f : Helper.NextFloat(5f, 10f);
        timerToDisplayGoMessage = Global.TEST_MODE ? 0f : TIME_TO_DISPLAY_GO_MESSAGE;
        timerDisplayRules = Global.TEST_MODE ? 0f : TIME_DISPLAY_RULES;

        timerToWaitBeforeRestart = TIME_TO_WAIT_BEFORE_RESTART;

        // Don't display the messages for "GO!" or "DRAW!" at all, until time warrants it. 
        displayGoMessage = false;
        displayRules = false;

        // Initialize the style for displaying the "GO!" and "DRAW!" messages.
        startMessageStyle = new GUIStyle();
        startMessageStyle.normal.textColor = new Color(173f / 255f, 216f / 255f, 230f / 255f);
        startMessageStyle.font = Global.STENCIL;
        startMessageStyle.fontSize = 100;
        startMessageStyle.alignment = TextAnchor.MiddleCenter;

        gameEndsMessageStyle = new GUIStyle();
        gameEndsMessageStyle.normal.textColor = Color.red;
        gameEndsMessageStyle.font = Global.STENCIL;
        gameEndsMessageStyle.fontSize = 150;
        gameEndsMessageStyle.alignment = TextAnchor.MiddleCenter;

        levelReloadingStyle = new GUIStyle();
        levelReloadingStyle.normal.textColor = Color.yellow;
        levelReloadingStyle.font = Global.STENCIL;
        levelReloadingStyle.fontSize = 50;
        levelReloadingStyle.alignment = TextAnchor.MiddleCenter;

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
        int numJoysticksAssigned = Math.Min(2, 2);

        for (int i = 0; i < 2; i++) {
            // Add the player script if it has not been added, and start it.
            if (playerList[i].GetComponent<Player>() == null) {
                playerList[i].AddComponent<Player>();
            }
        }

        // Generate a list of player scripts representing each of the players, and add
        // the Player components to the list.
        playerScripts = new List<Player>() {
        };
        playerList.ForEach(x => playerScripts.Add(x.GetComponent<Player>()));

        for (int i = 0; i < 2; i++) {
            playerScripts[i].PreStart(i + 1);
            playerScripts[i].symbolsToPress = new Queue<List<PS4Pressable>>();
        }

        // Populate the sequence of keys to be pressed with 20 items.
        for (int j = 0; j < 20; j++) {
            List<PS4Pressable> sequenceToAdd = Helper.GetRandomListOfKeysToPress();
            for (int i = 0; i < 2; i++) {
                playerScripts[i].symbolsToPress.Enqueue(sequenceToAdd);
            }
        }

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    // Update the match every frame.
    void Update()
    {
        if (playerScripts[0].symbolsToPress.Count <= 5 || playerScripts[1].symbolsToPress.Count <= 5) {
            for (int j = 0; j < 20; j++) {
                List<PS4Pressable> symbolsToAdd = Helper.GetRandomListOfKeysToPress();
                playerScripts[0].symbolsToPress.Enqueue(symbolsToAdd);
                playerScripts[1].symbolsToPress.Enqueue(symbolsToAdd);
            }
        }

        // Display the rules on-screen.
        if (timerDisplayRules > 0f) {
            displayRules = true;
            timerDisplayRules -= Time.deltaTime;
            startMessageStyle.fontSize = 80;
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

        // Once the timer to display the go message counts down, the game has begun!
        if (timerToDisplayGoMessage <= 0f) {

            if (!gameOver) {



                bool player1FiredBullet = playerScripts[0].queuedShots.Count != 0;
                bool player2FiredBullet = playerScripts[1].queuedShots.Count != 0;

                // Player 1 fires a bullet and player 2 doesn't.
                if (player1FiredBullet && !player2FiredBullet) {
                    playerScripts[0].queuedShots.Dequeue();

                    if (!playerScripts[0].isDodging) {
                        playerScripts[1].DecrementHP(1);
                        playerScripts[1].mostRecentStatusMessage = "OUCH!";
                        playerScripts[0].mostRecentStatusMessage = "HIT!";
                    }
                    else {
                        playerScripts[0].isDodging = false;
                        playerScripts[1].mostRecentStatusMessage = "DODGE!";
                        playerScripts[0].mostRecentStatusMessage = "MISSED!";
                    }
                }

                // Player 2 fires a bullet and player 1 doesn't.
                else if (!player1FiredBullet && player2FiredBullet) {

                    playerScripts[1].queuedShots.Dequeue();

                    if (!playerScripts[1].isDodging) {
                        playerScripts[0].DecrementHP(1);
                        playerScripts[0].mostRecentStatusMessage = "OUCH!";
                        playerScripts[1].mostRecentStatusMessage = "HIT!";
                    }
                    else {
                        playerScripts[1].isDodging = false;
                        playerScripts[0].mostRecentStatusMessage = "DODGE!";
                        playerScripts[1].mostRecentStatusMessage = "MISSED!";

                    }

                }

                else if (player1FiredBullet && player2FiredBullet) {
                    float player1Time = Math.Abs(playerScripts[0].queuedShots.Peek());
                    float player2Time = Math.Abs(playerScripts[1].queuedShots.Peek());

                    // Tied bullet strikes
                    if (Math.Abs(player1Time - player2Time) <= 0.05) {
                        playerScripts[1].queuedShots.Dequeue();
                        playerScripts[0].queuedShots.Dequeue();

                        playerScripts[0].mostRecentStatusMessage = "BLOCKED!";
                        playerScripts[1].mostRecentStatusMessage = "BLOCKED!";
                    }

                    // 
                    else if (player1Time > player2Time) {
                        playerScripts[1].queuedShots.Dequeue();

                        if (!playerScripts[1].isDodging) {
                            playerScripts[0].DecrementHP(1);
                            playerScripts[0].mostRecentStatusMessage = "OUCH!";
                            playerScripts[1].mostRecentStatusMessage = "HIT!";
                        }
                        else {
                            playerScripts[1].isDodging = false;
                        }
                    }

                    else if (player2Time > player1Time) {
                        playerScripts[0].queuedShots.Dequeue();

                        if (!playerScripts[0].isDodging) {
                            playerScripts[1].DecrementHP(1);
                            playerScripts[1].mostRecentStatusMessage = "OUCH!";
                            playerScripts[0].mostRecentStatusMessage = "HIT!";
                        }
                        else {
                            playerScripts[0].isDodging = false;
                            playerScripts[1].mostRecentStatusMessage = "DODGE!";
                            playerScripts[0].mostRecentStatusMessage = "MISSED!";
                        }
                    }

                }

                playerScripts[0].isDodging = false;
                playerScripts[1].isDodging = false;

                if (gameOver) {
                    CheckAndDisplayFinalScores();

                    // Wait 
                    if (timerToWaitBeforeRestart >= 0f) {
                        timerToWaitBeforeRestart -= Time.deltaTime;
                    }
                    else {
                        Application.LoadLevel(Application.loadedLevel);
                    }
                }


            }

            else if (gameOver) {
                CheckAndDisplayFinalScores();

                // Wait 
                if (timerToWaitBeforeRestart >= 0f) {
                    timerToWaitBeforeRestart -= Time.deltaTime;
                }
                else {
                    Application.LoadLevel(Application.loadedLevel);
                }
            }

        }

    }

    private void CheckAndDisplayFinalScores()
    {
        if (playerScripts[0].currentHP > 0 && playerScripts[1].currentHP <= 0) {
            gameVictoryStatus = GameVictoryStatus.PLAYER_1_WINS;
        }
        else if (playerScripts[0].currentHP <= 0 && playerScripts[1].currentHP > 0) {
            gameVictoryStatus = GameVictoryStatus.PLAYER_2_WINS;
        }
        else {
            gameVictoryStatus = GameVictoryStatus.DRAW;
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
                               START_MESSAGE_HEIGHT), "GET READY TO ATTACK...!", startMessageStyle);
        }

        // Display the message to begin shooting after somewhere between 5 to 10 seconds.
        if (displayGoMessage) {
            GUI.Label(new Rect((float)Screen.width / 2f - (START_MESSAGE_WIDTH / 2f),
                       (float)Screen.height / 2f - (START_MESSAGE_HEIGHT / 2f),
                       START_MESSAGE_WIDTH,
                       START_MESSAGE_HEIGHT), "DRAW!", startMessageStyle);
        }

        if (gameOver) {

            Rect finalDisplayRectangle = new Rect(Screen.width / 2f - 100f / 2f, Screen.height / 2f - 50f / 2f, 100f, 50f);

            switch (gameVictoryStatus) {
                case (GameVictoryStatus.PLAYER_1_WINS): {
                        finalVictoryMessage = "Player 1 wins!";
                        break;
                    }
                case (GameVictoryStatus.PLAYER_2_WINS): {
                        finalVictoryMessage = "Player 2 wins!";
                        break;
                    }
                case (GameVictoryStatus.DRAW): {
                        finalVictoryMessage = "Draw! Both players dead!";
                        break;
                    }
            }


            GUI.Label(finalDisplayRectangle, finalVictoryMessage, gameEndsMessageStyle);

            GUI.Label(levelReloadingRectangle, levelReloadingMessage, levelReloadingStyle);


        }


    }

}

enum GameVictoryStatus
{
    PLAYER_1_WINS,
    PLAYER_2_WINS,
    DRAW

}
