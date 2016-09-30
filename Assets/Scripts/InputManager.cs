using UnityEngine;
using System.Collections;

public class InputManager : Manager {
	public static InputManager main = null;

	private float inspectRadius = 0.1f;

	private bool lockInspect = false;
	private bool pendingDecision = false;

	private LayerMask layerMask;

	private CharacterPlayer player;

	#region initalize

	void OnEnable() {
		if(!main) main = this;
		else Destroy(gameObject);
		DontDestroyOnLoad (gameObject);
	}

	void Start() {
	}

	void Update() {
		MouseInput();
		KeyboardInput();
	}
		
	#endregion

	#region PrivateFunctions

	public static void LockInspection() {
		main.lockInspect = true;
	}

	public static void UnlockInspection() {
		main.lockInspect = false;
	}

	public static void LockPendingDecision() {
		main.pendingDecision = true;
	}

	public static void UnlockPendingDecision() {
		main.pendingDecision = false;
	}

	#endregion

	#region GameInput

	void KeyboardInput() {

		// Special Keys
		if (Input.GetButtonDown ("Pause")) {
			GameManager.main.PauseGame ();
		}
		if (GameManager.main.gamePaused)
			return;
		if (Input.GetButtonDown ("Inventory")) {
			UIManager.main.ToggleDarkenScreen ();
			UIManager.main.ToggleScreen("Inventory");
		}
		else if (Input.GetButtonDown ("Inspect")) {
			if (!lockInspect) {
				player = GameObject.FindWithTag ("Player").GetComponent<CharacterPlayer> ();
				if (!player) return;
				player.Inspect ();
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha0)) {
			if (pendingDecision) {
				Debug.LogWarning ("0 is not a valid decision for conversations decision");
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha1)) {
			if (pendingDecision) {
				player = GameObject.FindWithTag ("Player").GetComponent<CharacterPlayer> ();
				if (!player) return;
				player.DialogueDecision (1);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			if (pendingDecision) {
				player = GameObject.FindWithTag ("Player").GetComponent<CharacterPlayer> ();
				if (!player) return;
				player.DialogueDecision (2);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			if (pendingDecision) {
				player = GameObject.FindWithTag ("Player").GetComponent<CharacterPlayer> ();
				if (!player) return;
				player.DialogueDecision (3);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4)) {
			if (pendingDecision) {
				player = GameObject.FindWithTag ("Player").GetComponent<CharacterPlayer> ();
				if (!player) return;
				player.DialogueDecision (4);
			}
		}
	}

	void MouseInput() {
		
	}

	#endregion
}
