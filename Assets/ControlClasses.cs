// An enumeration of different axes types: is it a mouse movement, a key/mouse button press, or a joystick movement?
public enum AxisType
{
    KeyOrMouseButton = 0,
    MouseMovement = 1,
    JoystickAxis = 2
};

// An enumeration of all the different axes of control on a PS4 controller.
public enum PS4Control
{
    VERTICAL,
    HORIZONTAL,
    TRIANGLE,
    SQUARE,
    X,
    CIRCLE,
    L1,
    L2,
    R1,
    R2,
    HORIZONTAL_LEFT_STICK,
    HORIZONTAL_RIGHT_STICK,
    VERTICAL_LEFT_STICK,
    VERTICAL_RIGHT_STICK
}

// A class representation of all characteristics of an input axis.
public class InputAxis
{
    public string name;
    public string descriptiveName;
    public string descriptiveNegativeName;
    public string negativeButton;
    public string positiveButton;
    public string altNegativeButton;
    public string altPositiveButton;

    public float gravity;
    public float dead;
    public float sensitivity;

    public bool snap = false;
    public bool invert = false;

    public AxisType type;

    public int axis;
    public int joyNum;
}
