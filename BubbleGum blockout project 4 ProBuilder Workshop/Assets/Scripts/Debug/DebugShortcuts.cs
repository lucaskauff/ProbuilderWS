using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;

public class DebugShortcuts : SerializedMonoBehaviour {

	public enum DebugButton{JOYCON, AIR_CONTROL, CLEAR_TRACE, CLEAR_AI, RESPAWN, CHANGE_SCENE, DISPLAY_UI}
	public enum DebugUI{JOYCON, AIR_CONTROL, IN_AIR, IN_FALL, IMMUNITY}

	[SerializeField] private bool beginWithDebug = false;

	[OdinSerialize] private Dictionary<DebugButton, bool> useButtons = null;
	[OdinSerialize] private Dictionary<DebugUI, bool> useUis = null;

	[Header("References")]
	[SerializeField] private Animator debugAnimator = null;
	[OdinSerialize] private Dictionary<DebugUI, GameObject> uis = null;
	[OdinSerialize] private Dictionary<DebugUI, TMP_Text> uiValues = null;

	private PlayerCharacter character;

	private CustomInputManager inputManager;

	private AiSpawnTrigger[] spawnTriggers;

	private AiSystem aiSystem;

	private Respawn respawn;

	private void Awake() {
		inputManager = CustomInputManager.Instance;
		aiSystem = AiSystem.Instance;
		character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
		spawnTriggers = FindObjectsOfType<AiSpawnTrigger>();
		respawn = FindObjectOfType<Respawn>();

		foreach(DebugUI ui in useUis.Keys){
			if(uis.ContainsKey(ui) && (uis[ui] != null)){
				uis[ui].SetActive(useUis[ui]);
			}
		}

		if(beginWithDebug){
			debugAnimator.SetTrigger("Display");
		}
	}

	private void Update() {
		//if(UseButton(DebugButton.JOYCON) && inputManager.GetToggleJoyconsDown()){
		//	inputManager.ToggleJoycons();
		//}

		if(UseButton(DebugButton.AIR_CONTROL) && inputManager.GetAirControlButtonDown()){
			character.ChangeAirControl();
		}

		if(UseButton(DebugButton.CLEAR_TRACE) && inputManager.GetClearTraceDown()){
			character.PlayerComponents.Painter.ClearTraces();
		}

		if(UseButton(DebugButton.CLEAR_AI) && inputManager.GetClearAIsDown()){
			foreach (AiSpawnTrigger trigger in spawnTriggers)
			{
				trigger.ResetTrigger();
			}

			aiSystem.ClearAIs();
		}

		if(UseButton(DebugButton.RESPAWN) && inputManager.GetRespawnButtonDown()){
			respawn.DoRespawn();
		}

		if(UseButton(DebugButton.CHANGE_SCENE)){
			for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++){
				if((SceneManager.GetActiveScene().buildIndex != i) && Input.GetKeyDown(KeyCode.Keypad1 + i)){
					inputManager.UnLockAll();
					SceneManager.LoadScene(i);
				}
			}
		}

		if(UseButton(DebugButton.DISPLAY_UI) && inputManager.GetDisplayDebugDown() && (debugAnimator != null)){
			debugAnimator.SetTrigger("Display");
		}
		

		if(UseUI(DebugUI.JOYCON)){
			uiValues[DebugUI.JOYCON].text = inputManager.useJoycons.ToString();
		}

		if(UseUI(DebugUI.AIR_CONTROL)){
			uiValues[DebugUI.AIR_CONTROL].text = character.airControl.ToString();
		}

		if(UseUI(DebugUI.IN_AIR)){
			bool inAir = !character.BikeComponents.AirState.OnGround;
			inAir = inAir && !character.BikeComponents.AirState.InFall;
			uiValues[DebugUI.IN_AIR].text = inAir.ToString();
		}

		if(UseUI(DebugUI.IN_FALL)){
			uiValues[DebugUI.IN_FALL].text = character.BikeComponents.AirState.InFall.ToString();
		}
		if(UseUI(DebugUI.IMMUNITY)){
			uiValues[DebugUI.IMMUNITY].text = character.PlayerComponents.Strike.InImmunity.ToString();
		}
	}

	private bool UseButton(DebugButton button){
		return useButtons.ContainsKey(button) && useButtons[button];
	}

	private bool UseUI(DebugUI button){
		bool result = useUis.ContainsKey(button);
		result = result && uiValues.ContainsKey(DebugUI.JOYCON);
		result = result && (uiValues[button] != null);
		result = result && useUis[button];
		return result;
	}
}
