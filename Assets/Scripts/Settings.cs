using UnityEngine;
using System.Collections;



/// <summary>
/// Contains a list of settings for the game Macabre, belings in GameManager
/// </summary>
public struct Settings {
	public static bool reloadEverything = false;

	public static bool enableSaving = true;
	public static bool enableGameClock = false;
	public static float searchRadius = 0.2f;

	public static bool reAssertUIScreen = true;

	public static bool DebugCONVODatabaseListTF = false;

	public static int ConversationCharacterLimit = 500;
	public static int ConversationChoiceOptions = 4;

	public static int CharacterInventorySize = 20;

	public static bool debugCONVO = true;
	public static bool debugINSPECT = true;
	public static bool debugDATA = true;
	public static bool debugCAMERA = false;
	public static bool debugSAVE = false;

	public static float moveMovementSpeed = 3.0f;

	public static bool EnableMacabreTest = true;

}
