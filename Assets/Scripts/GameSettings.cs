/// <summary>
/// Contains a list of settings for the game Macabre, belings in GameManager
/// </summary>
public struct GameSettings {

    // Debug Stats
    public static bool enableSaving = true;
	public static bool enableGameClock = false;
    public static bool reAssertUIScreen = true;
    public static bool autoSerializeGame = true;

    // Game Stats
    public static int characterInventorySize = 20;
    public static int conversationCharacterLimit = 500;
    public static int conversationChoiceOptions = 4;
    public static float moveMovementSpeed = 3.0f;
    public static float inspectRadius = 0.2f;

    // Controls
    public static bool useKeyboardMovement = true;
    public static bool useMoouseMovement = false;
}
