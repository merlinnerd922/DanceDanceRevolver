using ExtendSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// An enumeration denoting whether a pressed button/axis is currently active (pressed) or inactive (unpressed).
public enum ButtonState
{
    INACTIVE,
    ACTIVE
}

// A class denoting a script attached to each player object.
public class Player : MonoBehaviour
{
    const int MAX_DODGES = 3; // Denotes the maximum number of times one can dodge a bullet.
    int currentDodgesLeft; // Denotes the number of opportunities that one has left to dodge a bullet.

    // This is the bullet prefab the will be instantiated when the player clicks
    GameObject newBullet, bullet;

    int playerNum; // Player's player number.
    string stringRepOnScreen; // On-screen representation of the player's control pressed.
    private GUIStyle controlDisplayStyle; // The font style of the display of the controls pressed.
    static int MAX_HP; // The max amount of HP for each player
    public int currentHP; // The HP left that each player has.
    public Queue<float> queuedShots; // A queue of shots to be fired at the opponent.

    // Represents the current queue of sequences of symbols that one currently has to press.
    public Queue<List<PS4Pressable>> symbolsToPress; 

    private bool sequenceIsNotActive; // A boolean indicating that there is currently no sequence of keys on-screen for the user to press.
    private List<PS4Pressable> currentActiveKeySequence; // A sequence indicating the current active sequence of keys to press.

    // Denotes how many symbols out of the number of symbols in the current sequence that the player is processing right now.
    public int numSymbolsPressed; 

    // Denotes a mapping between each control, and its previous and current button state.
    public Dictionary<PS4Control, Dictionary<string, ButtonState>> controlButtonStateMapping;

    // Mappings between player number and location of where to draw the number of HP left.
    public List<Rect> locationToDrawHPRemaining = new List<Rect>() {
        new Rect(50f, 50f, 100f, 50f),
        new Rect(Screen.width - 300f, Screen.height - 100f, 100f, 50f)
    };

    // Location of where to display all 3 - 5 symbols to press.
    public List<Rect> baseLocationToDrawImageSymbolSequence = new List<Rect>() {
        new Rect(50f, 100f, 50f, 50f),
        new Rect(Screen.width - 350f, Screen.height - 150f, 50f, 50f)
    };

    // Represents the location of where to draw the current status message about information such as whether
    // the player has successfully shot the opponent, has successully dodged an opponent etc.
    public Rect locationToDrawStatusMessage
    {
        // Convert the player's world position to screen coordinates, and adjust the information so that 
        // it's above the player's head. Then return the rectangle as to where that information should be displayed.
        get
        {
            Vector3 player1ScreenPos = MatchManager.mainCamera.WorldToScreenPoint(this.gameObject.transform.position);
            player1ScreenPos.y = Screen.height - player1ScreenPos.y;
            return new Rect(player1ScreenPos.x - 40, player1ScreenPos.y - 110, 100, 50);   
        }
    }

    // Represents the location of where to draw the number of dodges a player has left.
    public List<Rect> locationToDrawDodgesLeft = new List<Rect>() {
        new Rect(50f, 150f, 100f, 50f),
        new Rect(Screen.width - 400f , Screen.height - 200f, 100f, 50f)
    };

    // A mapping of pressable characters to their string representations on-screen.
    public static Dictionary<PS4Pressable, string> pressableCharacterSymbolMapping = new Dictionary<PS4Pressable, string>() {
        { PS4Pressable.L1, "L1" },
        { PS4Pressable.L2, "L2" },
        { PS4Pressable.R1, "R1" },
        { PS4Pressable.R2, "R2" },
        { PS4Pressable.TRIANGLE, "T" }, 
        { PS4Pressable.SQUARE, "S" },
        { PS4Pressable.CIRCLE, "C" },
        { PS4Pressable.X, "X"}
    };

    // Denotes mappings between pressable buttons and the locations of their corresponding images.
    public static Dictionary<PS4Pressable, Texture> pressableImageMapping = new Dictionary<PS4Pressable, Texture>() {
        { PS4Pressable.L1, Resources.Load<Texture>("DuelloIcons/L1") },
        { PS4Pressable.L2, Resources.Load<Texture>("DuelloIcons/L2") },
        { PS4Pressable.R1, Resources.Load<Texture>("DuelloIcons/R1") },
        { PS4Pressable.R2, Resources.Load<Texture>("DuelloIcons/R2") },
        { PS4Pressable.TRIANGLE, Resources.Load<Texture>("DuelloIcons/triangle") }, 
        { PS4Pressable.SQUARE, Resources.Load<Texture>("DuelloIcons/square") },
        { PS4Pressable.CIRCLE, Resources.Load<Texture>("DuelloIcons/circle") },
        { PS4Pressable.X, Resources.Load<Texture>("DuelloIcons/ex")}
    };

    // A mapping between the different pressable axes and the current and previous button states
    // for those axes.
    private Dictionary<PS4Pressable, Dictionary<string, ButtonState>> controlAxisStateMapping;
    
    public string mostRecentStatusMessage; // A message detailing the most recent happenings of this player.
    private GUIStyle playerStatusDisplay; // A font styling detailing the most recent happenings to this player.

    // Textures containing graphic representations for bullet clashing, making a mark and dodging a bullet.
    public Texture clashGraphic;
    private Texture hitGraphic;
    private Texture dodgeGraphic;
    
    public bool isDodging; // Indicates whether the player is currently dodging a bullet.

    // Call this special method to load characteristics associated with this player.
    public void PreStart(int _playerNum)
    {
        MAX_HP = Global.TEST_MODE ? 1 : 7; // Set test map HP to 1 in test mode.

        bullet = Resources.Load<GameObject>("Bullet");
        playerNum = _playerNum; 
        currentDodgesLeft = MAX_DODGES;

        // Create style for displaying info on the control most recently pressed.
        controlDisplayStyle = new GUIStyle();
        controlDisplayStyle.normal.textColor = new Color(173f / 255f, 216f/ 255f, 230f/ 255f);
        controlDisplayStyle.font = Global.STENCIL;
        controlDisplayStyle.fontSize = 50;

        // Create a style for displaying info on the most information on the player's status.
        playerStatusDisplay = new GUIStyle();
        playerStatusDisplay.normal.textColor = new Color(173f / 255f, 216f/ 255f, 230f/ 255f);
        playerStatusDisplay.font = Global.STENCIL;
        playerStatusDisplay.fontSize = 50;
        playerStatusDisplay.alignment = TextAnchor.MiddleCenter;

        // The player starts off with MAX_HP hit points.
        currentHP = MAX_HP;

        // Initialize the queue of shots to be fired at the opponent.
        queuedShots = new Queue<float>();

        // Mappings between axes and their current and previous states.
        controlButtonStateMapping = Helper.PS4Buttons.ToDictionary(x => x, x => new Dictionary<string, ButtonState>() {
            { "currentButtonState", ButtonState.INACTIVE }, 
            { "prevButtonState", ButtonState.INACTIVE }
        });

        controlAxisStateMapping = Helper.PS4AxesPressables.ToDictionary(x => x, x => new Dictionary<string, ButtonState>() {
            { "currentButtonState", ButtonState.INACTIVE }, 
            { "prevButtonState", ButtonState.INACTIVE }
        });

        mostRecentStatusMessage = ""; // At the start, nothing recent has been happening so this is blank.

        // Represents the current sequence of keys the player is trying to match.
        currentActiveKeySequence = new List<PS4Pressable>() { };

        numSymbolsPressed = 0;

        clashGraphic = Resources.Load<Texture>("DuelloIcons/clash");
        hitGraphic = Resources.Load<Texture>("DuelloIcons/hit");
        dodgeGraphic = Resources.Load<Texture>("DuelloIcons/dodge");

        isDodging = false;
        stringRepOnScreen = "";

        currentDodgesLeft = MAX_DODGES;

    }

    // Fire one shot at the opponent.
    void FireOneShot()
    {
        Bullet newBulletComponent;

        // Add a timed bullet shot to the queue.
        queuedShots.Enqueue(Time.deltaTime);

        // Create and instantiate a new bullet, fire it from the player.
        newBullet = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        newBulletComponent = newBullet.GetComponent<Bullet>();
        if (newBulletComponent == null) {
            newBullet.AddComponent<Bullet>();
            newBulletComponent = newBullet.GetComponent<Bullet>();
        }
        
        newBulletComponent.PreStart();
        newBulletComponent.Fire();
    }

    // Update the player control.
    void Update()
    {
        if (MatchManager.matchHasBegun) {

            // Get the next sequence if there currently is no active sequence.
            if (currentActiveKeySequence.Count == 0) {
                currentActiveKeySequence = symbolsToPress.Dequeue();
            }

            // Update the player to reflect pressed input controls.
            bool inputBoolVal;

            // Check each given possible control button/axis to see if it's been pressed/toggled.
            foreach (PS4Control c in Helper.PS4Controls) {


                if (c.IsAny(PS4Control.L3, PS4Control.R3)) {

                    inputBoolVal = Input.GetButton(Helper.GetPlayerAxis(playerNum, c));
                    if (inputBoolVal) {
                        stringRepOnScreen = c.ToString();
                        if (controlButtonStateMapping[c]["currentButtonState"] == ButtonState.INACTIVE) {
                            controlButtonStateMapping[c]["currentButtonState"] = ButtonState.ACTIVE;
                            controlButtonStateMapping[c]["prevButtonState"] = ButtonState.INACTIVE;
                        }
                    }
                    else {
                        if (controlButtonStateMapping[c]["currentButtonState"] == ButtonState.ACTIVE) {
                            controlButtonStateMapping[c]["currentButtonState"] = ButtonState.INACTIVE;
                            controlButtonStateMapping[c]["prevButtonState"] = ButtonState.ACTIVE;

                            if (currentDodgesLeft > 0) {
                                mostRecentStatusMessage = "DODGE!";
                                currentDodgesLeft--;
                                isDodging = true;
                            }

                        }
                    }

                }

                // For buttons, check to see if they've been pressed, and if so, update the most recently pressed control.
                if (c.IsButton()) {

                    inputBoolVal = Input.GetButton(Helper.GetPlayerAxis(playerNum, c));
                    if (inputBoolVal) {
                        stringRepOnScreen = c.ToString();
                        if (controlButtonStateMapping[c]["currentButtonState"] == ButtonState.INACTIVE) {
                            controlButtonStateMapping[c]["currentButtonState"] = ButtonState.ACTIVE;
                            controlButtonStateMapping[c]["prevButtonState"] = ButtonState.INACTIVE;
                        }
                    }
                    else {

                        if (controlButtonStateMapping[c]["currentButtonState"] == ButtonState.ACTIVE) {
                            controlButtonStateMapping[c]["currentButtonState"] = ButtonState.INACTIVE;
                            controlButtonStateMapping[c]["prevButtonState"] = ButtonState.ACTIVE;
                            if (currentActiveKeySequence[numSymbolsPressed] == c.ToPS4Pressable()) {
                                numSymbolsPressed += 1;
                                if (numSymbolsPressed == currentActiveKeySequence.Count) {
                                    currentActiveKeySequence = new List<PS4Pressable>() {
                                    };
                                    FireOneShot();
                                    numSymbolsPressed = 0;
                                }
                            }
                            else {
                                numSymbolsPressed = 0;
                            }
                        }

                    }
                }
            }

        }
    }

    // Draw everything every frame.
    void OnGUI()
    {
        // On every frame, display the control that was recently pressed.
        GUI.Label(new Rect(0, Screen.height - 50, 50, 50), stringRepOnScreen, controlDisplayStyle);

        // Display the amount of HP each player has.
        GUI.Label(locationToDrawHPRemaining[playerNum - 1], String.Format("HP: {0} / {1}", currentHP.ToString(),
                  MAX_HP.ToString()), controlDisplayStyle);

        // Display the message associating the status of the most recently used bullet.
		if (mostRecentStatusMessage == "BLOCKED!") {
            GUI.DrawTexture(locationToDrawStatusMessage, clashGraphic, ScaleMode.StretchToFill, true);
        }
        else if (mostRecentStatusMessage == "HIT!") {
            GUI.DrawTexture(locationToDrawStatusMessage, hitGraphic, ScaleMode.StretchToFill, true);
        }
        else if (mostRecentStatusMessage == "DODGE!") {
            GUI.DrawTexture(locationToDrawStatusMessage, dodgeGraphic, ScaleMode.StretchToFill, true);
        }
        else if (mostRecentStatusMessage != "") {
            GUI.Label(locationToDrawStatusMessage, mostRecentStatusMessage, playerStatusDisplay);
        }

        // Display the message displaying the current sequence of symbols to select.
        int j = 0;
        foreach (PS4Pressable symbol in currentActiveKeySequence) {

            // If this symbol has not been pressed yet, then draw the symbol. Make sure to increment the x by a multiple of
            // 50 pixels
            if (j >= numSymbolsPressed) {
                GUI.DrawTexture(baseLocationToDrawImageSymbolSequence[playerNum - 1].AddToX(50 * (j - numSymbolsPressed)), pressableImageMapping[symbol]);
            }
            j++;
        }

        // Display the number of dodges on screen.
        GUI.Label(locationToDrawDodgesLeft[playerNum - 1], String.Format("Dodges left: {0}", currentDodgesLeft), controlDisplayStyle);
    }

    // Decrement this player's HP by <p> points, but don't go below 0.
    internal void DecrementHP(int p)
    {
        currentHP = Math.Max(currentHP - p, 0);
    }
}
