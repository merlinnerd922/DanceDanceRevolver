using ExtendSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ButtonState
{
    INACTIVE,
    ACTIVE
}

// A class denoting a script attached to each player object.
public class Player : MonoBehaviour
{
    const int MAX_DODGES = 3;
    int dodgesLeft = 3;

    // This is the bullet prefab the will be instantiated when the player clicks
    GameObject newBullet, bullet;

    int playerNum; // Player's player number.
    string stringRepOnScreen; // On-screen representation of the player's control pressed.
    private GUIStyle controlDisplayStyle; // The font style of the display of the controls pressed.
    static int MAX_HP; // The max amount of HP for each player
    public int currentHP; // The HP left that each player has.
    public Queue<float> queuedShots; // A queue of shots to be fired at the opponent.
    public Queue<List<PS4Pressable>> symbolsToPress;

    private bool sequenceIsNotActive; // A boolean indicating that there is currently no sequence of keys on-screen for the user to press.
    private List<PS4Pressable> currentActiveKeySequence; // A sequence indicating the current active sequence of keys to press.

    public int numSymbolsPressed;

    public Dictionary<PS4Control, Dictionary<string, ButtonState>> controlButtonStateMapping;

    // Mappings between player number and location of where to draw the number of HP left.
    public List<Rect> locationToDrawHPRemaining = new List<Rect>() {
        new Rect(50f, 50f, 100f, 50f),
        new Rect(Screen.width - 300f, Screen.height - 100f, 100f, 50f)
    };

    // Location of where to display all 3 - 5 symbols to press.
    public List<Rect> locationToDrawSymbolSequence = new List<Rect>() {
        new Rect(50f, 100f, 100f, 50f),
        new Rect(Screen.width - 350f, Screen.height - 150f, 100f, 50f)
    };

    // Location of where to display all 3 - 5 symbols to press.
    public List<Rect> baseLocationToDrawImageSymbolSequence = new List<Rect>() {
        new Rect(50f, 100f, 50f, 50f),
        new Rect(Screen.width - 350f, Screen.height - 150f, 50f, 50f)
    };

    public Rect locationToDrawStatusMessage
    {
        get
        {
            Vector3 player1ScreenPos = MatchManager.mainCamera.WorldToScreenPoint(this.gameObject.transform.position);
            player1ScreenPos.y = Screen.height - player1ScreenPos.y;
            return new Rect(player1ScreenPos.x - 40, player1ScreenPos.y - 110, 100, 50);
            
        }
    
    }

    public List<Rect> locationToDrawDodgesLeft = new List<Rect>() {
        new Rect(50f, 150f, 100f, 50f),
        new Rect(Screen.width - 400f , Screen.height - 200f, 100f, 50f)
    };

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

    // Denotes mappings between pressable buttons and the locations of images.
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

    private Dictionary<PS4Pressable, Dictionary<string, ButtonState>> controlAxisStateMapping;
    public string mostRecentStatusMessage;

    public Texture clashGraphic;
    private Texture hitGraphic;
    public bool isDodging;
    private Texture dodgeGraphic;
    private GUIStyle playerStatusDisplay;

    // Call this special method to load characteristics associated with this player.
    public void PreStart(int _playerNum)
    {
        MAX_HP = Global.TEST_MODE ? 1 : 7;

        bullet = Resources.Load<GameObject>("Bullet");
        playerNum = _playerNum; // Player number
        dodgesLeft = MAX_DODGES;

        // Create style for displaying info on the control most recently pressed.
        controlDisplayStyle = new GUIStyle();
        controlDisplayStyle.normal.textColor = new Color(173f / 255f, 216f/ 255f, 230f/ 255f);
        controlDisplayStyle.font = Global.STENCIL;
        controlDisplayStyle.fontSize = 50;

        playerStatusDisplay = new GUIStyle();
        playerStatusDisplay.normal.textColor = new Color(173f / 255f, 216f/ 255f, 230f/ 255f);
        playerStatusDisplay.font = Global.STENCIL;
        playerStatusDisplay.fontSize = 50;
        playerStatusDisplay.alignment = TextAnchor.MiddleCenter;

        // The player starts off with MAX_HP hit points.
        currentHP = MAX_HP;

        // Initialize the queue of shots to be fired at the opponent.
        queuedShots = new Queue<float>();


        controlButtonStateMapping = Helper.PS4Buttons.ToDictionary(x => x, x => new Dictionary<string, ButtonState>() {
            { "currentButtonState", ButtonState.INACTIVE }, 
            { "prevButtonState", ButtonState.INACTIVE }
        });

        controlAxisStateMapping = Helper.PS4AxesPressables.ToDictionary(x => x, x => new Dictionary<string, ButtonState>() {
            { "currentButtonState", ButtonState.INACTIVE }, 
            { "prevButtonState", ButtonState.INACTIVE }
        });


        mostRecentStatusMessage = "";

        currentActiveKeySequence = new List<PS4Pressable>() {
        };

        numSymbolsPressed = 0;

        clashGraphic = Resources.Load<Texture>("DuelloIcons/clash");
        hitGraphic = Resources.Load<Texture>("DuelloIcons/hit");
        dodgeGraphic = Resources.Load<Texture>("DuelloIcons/dodge");

        isDodging = false;
        stringRepOnScreen = "";

    }

    // Fire one shot at the opponent.
    void FireOneShot()
    {
        Bullet newBulletComponent;

        // Add a fired bullet to the queue.
        queuedShots.Enqueue(Time.deltaTime);
        newBullet = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        if (newBullet.GetComponent<Bullet>() == null) {
            newBulletComponent = newBullet.AddComponent<Bullet>();
        }
        newBulletComponent = newBullet.GetComponent<Bullet>();
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
            float inputVal, inputValAbs;
            bool inputBoolVal;

            // Check each given possible control button/axis to see if it's been pressed/toggled.
            foreach (PS4Control c in Enum.GetValues(typeof(PS4Control))) {

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

                            if (dodgesLeft > 0) {
                                mostRecentStatusMessage = "DODGE!";
                                dodgesLeft--;
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

            //else {
            //    //inputBoolVal = Input.GetButton(Helper.GetPlayerAxis(playerNum, c));
            //    inputVal = Input.GetAxis(Helper.GetPlayerAxis(playerNum, c));

            //    // If the amount the control has been toggled is above the threshold:
            //    if (true) {

            //        if (!c.IsStick()) {

            //            // Display the info if the control manipulated was NOT a stick.
            //            stringRepOnScreen = c.ToString();

            //            if (c == PS4Control.VERTICAL) {
            //                if (inputVal > 0) {
            //                    if (!VertAxisInUse) {

            //                        if (controlAxisStateMapping[PS4Pressable.UP]["currentButtonState"] == ButtonState.INACTIVE) {
            //                            controlAxisStateMapping[PS4Pressable.UP]["currentButtonState"] = ButtonState.ACTIVE;
            //                            controlAxisStateMapping[PS4Pressable.UP]["prevButtonState"] = ButtonState.INACTIVE;
            //                            FireOneShot();
            //                        }
            //                        VertAxisInUse = true;
            //                    }
            //                }
            //                else if (inputVal < 0) {
            //                    if (!VertAxisInUse) {
            //                        if (controlAxisStateMapping[PS4Pressable.DOWN]["currentButtonState"] == ButtonState.INACTIVE) {
            //                            controlAxisStateMapping[PS4Pressable.DOWN]["currentButtonState"] = ButtonState.ACTIVE;
            //                            controlAxisStateMapping[PS4Pressable.DOWN]["prevButtonState"] = ButtonState.INACTIVE;
            //                            FireOneShot();
            //                        }
            //                        VertAxisInUse = true;
            //                    }
            //                }
            //                else {
            //                    VertAxisInUse = false;
            //                }
            //            }

            //        }

            //        // Otherwise, check if the left stick has been toggled, and if so, update the most recently pressed control to 
            //        // indicate the left stick has been pressed.

            //        // If the left stick has been moved vertically (more than it has horizontally):
            //        else if (c == PS4Control.VERTICAL_LEFT_STICK) {
            //            if (Math.Abs(inputVal) > Math.Abs(Input.GetAxis(Helper.GetPlayerAxis(playerNum, PS4Control.HORIZONTAL_LEFT_STICK)))) {
            //                stringRepOnScreen = c.ToString();
            //                break;
            //            }
            //        }

            //        // If the left stick has been moved horizontally (more than it has vertically):
            //        else if (c == PS4Control.HORIZONTAL_LEFT_STICK) {
            //            if (Math.Abs(inputVal) > Math.Abs(Input.GetAxis(Helper.GetPlayerAxis(playerNum, PS4Control.VERTICAL_LEFT_STICK)))) {
            //                stringRepOnScreen = c.ToString();
            //                break;
            //            }
            //        }

            //    }
            //    else {

            //        foreach (PS4Pressable psp in Helper.PS4AxesPressables) {
            //            if (controlAxisStateMapping[psp]["currentButtonState"] == ButtonState.ACTIVE) {
            //                controlAxisStateMapping[psp]["currentButtonState"] = ButtonState.INACTIVE;
            //                controlAxisStateMapping[psp]["prevButtonState"] = ButtonState.ACTIVE;
            //            }
            //        }




            //    }



            //}
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
        //GUI.Label(locationToDrawSymbolSequence[playerNum - 1], currentActiveKeySequence.ToOnScreenRep(numSymbolsPressed), controlDisplayStyle);
        int j = 0;
        foreach (PS4Pressable symbol in currentActiveKeySequence) {
            if (j >= numSymbolsPressed) {
                GUI.DrawTexture(baseLocationToDrawImageSymbolSequence[playerNum - 1].AddToX(50 * (j - numSymbolsPressed)), pressableImageMapping[symbol]);
            }
            j++;
        }

        // Display the number of dodges on screen.
        GUI.Label(locationToDrawDodgesLeft[playerNum - 1], String.Format("Dodges left: {0}", dodgesLeft), controlDisplayStyle);
    }


    internal void DecrementHP(int p)
    {
        currentHP = Math.Max(currentHP - p, 0);
    }
}
