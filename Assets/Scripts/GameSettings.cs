/// <summary>
/// Contains a list of settings for the game Macabre, belings in GameManager
/// </summary>
public struct GameSettings {

    // Debug Stats
    public static bool createNewGame = false;
    public static bool useSavedXMLConfiguration = false;
    public static bool createNewXMLConfiguration = true;
    public static bool enableSaving = true;
	public static bool enableGameClock = false;
    public static bool reAssertUIScreen = true;
    public static bool autoSerializeGame = true;

    // Environment Stats
    public static float cameraSpeed = 32.0f;
    public static int clockRate = 60;

    // Character Stats
    public static int characterInventorySize = 20;
    public static int conversationCharacterLimit = 500;
    public static int conversationChoiceOptions = 4;
    public static float characterWalkingSpeed = 20.0f;
    public static float characterRunningSpeed = 80.0f;
    public static float inspectRadius = 16.0f;

    // Item stats
    public static float dropDistance = 9.0f;

    // Controls
    public static bool useKeyboardMovement = true;
    public static bool useMouseMovement = false;
}
