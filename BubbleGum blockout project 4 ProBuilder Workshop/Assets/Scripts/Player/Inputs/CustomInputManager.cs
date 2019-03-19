using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum LockInputType {MENU, KINEMATIC, MOVEMENT, ROTATION}

[RequireComponent(typeof(BellInput), typeof(CustomRumble))]
public class CustomInputManager : SerializedMonoBehaviour {

	public enum JoyconAxe {RIGHT, UP, FORWARD}
	public enum JoyconType {LEFT, RIGHT}
	public enum JoyconLink {LEFT, RIGHT, BOTH}
	public enum InputAsk {NONE, LEFT, RIGHT, BOTH}

	[System.Serializable]
	private struct JoyconInput{
		public JoyconLink type;
		public Joycon.Button button;

		public JoyconInput(JoyconLink type, Joycon.Button button){
			this.type = type;
			this.button = button;
		}
	}

	public static CustomInputManager Instance {get; private set;}

	[Header("Movement Motions")]

	[Range(0f, 1f)]
	[SerializeField] private float minDeadZone = 0.045f;

	[Range(0f, 1f)]
	[SerializeField] private float smallestMaxDeadZone = 0f;

	[Range(0f, 1f)]
	[SerializeField] private float largestMaxDeadZone = 0f;

	[MinValue(1)]
	[SerializeField] private int nbSensibilityLevel = 5;

	[MinValue(1)]
	[SerializeField] private int firstLevel = 3;

	[SerializeField] private AnimationCurve motionCurve = null;

	[Header("Other Motions")]

	[SerializeField] JoyconMotions boostMotions = new JoyconMotions();
	[SerializeField] JoyconMotions airTrickMotions = new JoyconMotions();

	[Header("Joycon")]
	public bool useJoycons = true;
	[SerializeField] private bool useLeftHand = false;
	[SerializeField] private JoyconAxe mainJoyconAxe = JoyconAxe.FORWARD;
	private Dictionary<JoyconType, Color> joyconColors = null;

	private Dictionary<JoyconType, Joycon.Button> handUpButtons = null;

	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] forwardButtons = null;

	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] brakeButtons = null;

	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] airControlButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] respawnButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] clearEnemyButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] boostButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] ungrabButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] beginFakeJoyconsButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] quitButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] restartButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] testButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] sensibilityUpButtons = null;
	[SerializeField, TabGroup("Unique buttons")]
	private JoyconInput[] sensibilityDownButtons = null;
	
//	public JoyconManager JoyconManager {get; private set;}
	//private Player player = null;

	
	public CustomRumble InputVibrations {get; private set;}
	public Dictionary<JoyconType, Joycon> Joycons {get; private set;}

	private Dictionary<LockInputType, List<MonoBehaviour>> lockedInputs = null;


	private BellInput bellInput = null;

	private Transform joyconSimulator = null;

	private float horizontalMultiply = 0f;

	private JoyconType mainHand = JoyconType.LEFT;

	private float actualLevel;
	private float actualDeadZone = 0f;

	private void Awake() {
		if(Instance != null){
			if(Instance != this){
				Destroy(gameObject);
			}

			return;
		}

		DontDestroyOnLoad(this);
		Instance = this;

		lockedInputs = new Dictionary<LockInputType, List<MonoBehaviour>>();
		bellInput = GetComponent<BellInput>();
		InputVibrations = GetComponent<CustomRumble>();

//		Joycons = new Dictionary<JoyconType, Joycon>();

//		JoyconManager = gameObject.AddComponent<JoyconManager>();
//		JoyconManager.InitManager();

//		List<Joycon> tempList = JoyconManager.joyConList;

		//if(tempList == null){
		//	Debug.LogError("No Joycon List");
		//}
		//else{
		//	if (tempList.Count < 2){
		//		Debug.LogWarning("Need two joycons");
		//	}
		//	else{
		//		if(tempList[0].isLeft){
		//			Joycons.Add(JoyconType.LEFT, tempList[0]);
		//		}
		//		else{
		//			Joycons.Add(JoyconType.RIGHT, tempList[0]);
		//		}

		//		if(tempList[1].isLeft){
		//			if(Joycons.ContainsKey(JoyconType.LEFT)){
		//				Debug.LogWarning("Same Joycons");
		//			}
		//			else{
		//				Joycons.Add(JoyconType.LEFT, tempList[1]);
		//			}
		//		}
		//		else{
		//			if(Joycons.ContainsKey(JoyconType.RIGHT)){
		//				Debug.LogWarning("Same Joycons");
		//			}
		//			else{
		//				Joycons.Add(JoyconType.RIGHT, tempList[1]);
		//			}
		//		}
		//	}
		//}

		joyconSimulator = new GameObject("Joycon Simulator").GetComponent<Transform>();
		joyconSimulator.parent = GetComponent<Transform>();

		ChangeMainHand(useLeftHand);

		actualLevel = firstLevel;
		CalculateSensibility();
	}

	private void Start() {
		//player = ReInput.players.GetPlayer(0);
	}

	private void Update() {
		RecalibrateJoycon();


		if(!useJoycons || (Joycons.Count != 2)){
			return;
		}

		joyconSimulator.rotation = Joycons[mainHand].GetVector();
		
		if(boostMotions.useMotions || airTrickMotions.useMotions){
			Vector3 left = Joycons[JoyconType.LEFT].GetAccel();
			Vector3 right = Joycons[JoyconType.RIGHT].GetAccel();

			left = new Vector3(left.z, left.x, left.y);
			right = new Vector3(right.z, right.x, right.y);

			if(boostMotions.useMotions){
				boostMotions.UpdateMotions(left, right, Time.deltaTime);
			}

			if(airTrickMotions.useMotions){
				airTrickMotions.UpdateMotions(left, right, Time.deltaTime);
			}
		}
	}

	public void ToggleJoycons(){
		useJoycons = !useJoycons;
		foreach(JoyconType j in Joycons.Keys){
			Joycons[j].SetRumble(0f, 0f, 0f);
		}
	}

	#region INPUT_Lock

	public void Lock(LockInputType lockType, MonoBehaviour locker){
		if(!lockedInputs.ContainsKey(lockType)){
			lockedInputs.Add(lockType, new List<MonoBehaviour>());
		}

		if(!lockedInputs[lockType].Contains(locker)){
			lockedInputs[lockType].Add(locker);
		}
	}

	public void UnLock(LockInputType lockType, MonoBehaviour locker){
		if(lockedInputs.ContainsKey(lockType)){
			if(lockedInputs[lockType].Contains(locker)){
				lockedInputs[lockType].Remove(locker);
			}
		}
	}

	public void UnLockAll(){
		lockedInputs.Clear();
	}

	public bool IsLocked(LockInputType lockType){
		return (lockedInputs.ContainsKey(lockType) && (lockedInputs[lockType].Count > 0));
	}

	public bool IsOneLocked(LockInputType[] lockTypes){
		foreach(LockInputType type in lockTypes){
			if(IsLocked(type)){
				return true;
			}
		}
		
		return false;
	}

	#endregion
	
	#region INPUT_Movement

	public float GetJoyconRawHorizontal(){
		if(!useJoycons || (Joycons.Count < 2)){
			return 0f;
		}

		return horizontalMultiply * JoyconInputCalculator.GetHorizontalOneAxe(joyconSimulator, mainJoyconAxe);
	}

	public float GetHorizontal(){
		if(IsOneLocked(new LockInputType[]{LockInputType.MENU, LockInputType.KINEMATIC, LockInputType.MOVEMENT})){
			return 0f;
		}

		float keyboardValue = Input.GetAxis("Horizontal");

		if(!useJoycons || (Joycons.Count < 2) || (keyboardValue != 0f)){
			return keyboardValue;
		}

		float value = horizontalMultiply * JoyconInputCalculator.GetHorizontalOneAxe(joyconSimulator, mainJoyconAxe);
		
		float sign = Mathf.Sign(value);
		float abs = Mathf.Abs(value);
		if(abs < minDeadZone){
			return 0f;
		}

		if(abs > actualDeadZone){
			return sign;
		}

		return sign * motionCurve.Evaluate((abs - minDeadZone) / (actualDeadZone - minDeadZone));
		// return JoyconInputCalculator.GetHorizontal(joycons[JoyconType.RIGHT]);
	}

	public bool GetForwardButton(){
		if(IsOneLocked(new LockInputType[]{LockInputType.MENU, LockInputType.KINEMATIC, LockInputType.ROTATION})){
			return false;
		}

		bool forward = Input.GetButton("Forward");
		if(!useJoycons || forward){
			return forward;
		}

		return GetJoyconButton(forwardButtons);
	}

	public bool GetBrakeButton(){
		if(IsOneLocked(new LockInputType[]{LockInputType.MENU, LockInputType.KINEMATIC, LockInputType.ROTATION})){
			return false;
		}

		bool brake = Input.GetButton("Brake");
		if(!useJoycons || brake){
			return brake;
		}

		return GetJoyconButton(brakeButtons);
	}

	#endregion

	#region INPUT_InstantButtons

	public bool GetUngrabDown(){
		bool ungrab = Input.GetButtonDown("Ungrab");
		if(!useJoycons || ungrab){
			return ungrab;
		}

		return GetJoyconButtonDown(ungrabButtons);
	}

	public bool GetQuitButtonDown(){
		bool quit = Input.GetButtonDown("Quit");
		if(!useJoycons || quit){
			return quit;
		}

		return GetJoyconButtonDown(quitButtons);
	}

	public bool GetRestartButtonDown(){
		bool restart = Input.GetButtonDown("Restart");
		if(!useJoycons || restart){
			return restart;
		}

		return GetJoyconButtonDown(restartButtons);
	}

	#endregion

	#region INPUT_ContinueButtons

	public bool GetBoost(){
		bool boost = Input.GetButton("Boost");
		if(!useJoycons || boost){
			return boost;
		}

		if(boostMotions.useMotions){
			boost = boostMotions.InMotions();
		}

		return boost || GetJoyconButton(boostButtons);
	}

	public bool GetLeftAirTrick(){
		bool airTrick = Input.GetButton("AirTrick");
		if(!useJoycons || !airTrickMotions.useMotions || airTrick){
			return airTrick;
		}

		return airTrickMotions.IsLeftMotion;
	}

	public bool GetRightAirTrick(){
		bool airTrick = Input.GetButton("AirTrick");
		if(!useJoycons || !airTrickMotions.useMotions || airTrick){
			return airTrick;
		}

		return airTrickMotions.IsRightMotion;
	}

	#endregion

	#region INPUT_SeparateButtons

	public void RecalibrateJoycon(){
		//foreach(var joycon in Joycons){
		//	foreach(var button in recalibrateButtons){
		//		if(joycon.Value.GetButtonDown(button.Value)){
		//			if(joycon.Key == JoyconType.LEFT){
		//				if(button.Key == JoyconType.LEFT){
		//					Debug.Log("Recalibre Left Joycon");
		//					joycon.Value.Recenter();
		//					return;
		//				}
		//			}
		//			else{
		//				if(button.Key == JoyconType.RIGHT){
		//					Debug.Log("Recalibre Right Joycon");
		//					joycon.Value.Recenter();
		//					return;
		//				}
		//			}
		//		}
		//	}
		//}
	}

	public InputAsk GetHandUp(){
		bool left = Input.GetButton("LeftHighFive");
		bool right = Input.GetButton("RightHighFive");

		if(left && right){
			return InputAsk.BOTH;
		}
		else if(left){
			return InputAsk.LEFT;
		}
		else if(right){
			return InputAsk.RIGHT;
		}
		else if(!useJoycons){
			return InputAsk.NONE;
		}

		InputAsk result = InputAsk.NONE;
		
		foreach(var joycon in Joycons){
			foreach(var button in handUpButtons){
				if(joycon.Value.GetButton(button.Value)){
					if(joycon.Key == JoyconType.LEFT){
						if(button.Key == JoyconType.LEFT){
							if(result != InputAsk.NONE){
								result = InputAsk.BOTH;
							}
							else{
								result = InputAsk.LEFT;
							}
						}
					}
					else{
						if(button.Key == JoyconType.RIGHT){
							if(result != InputAsk.NONE){
								result = InputAsk.BOTH;
							}
							else{
								result = InputAsk.RIGHT;
							}
						}
					}
				}
			}
		}

		return result;
	}

	#endregion

	#region OtherInputs

	public bool GetBellDown(){
		bool bell = Input.GetButtonDown("Bell");
		if(!useJoycons || bell){
			return bell;
		}

		return bellInput.BellDown;
	}

	#endregion

	#region DebugInputs

	public bool GetTestBool(){
		bool test = Input.GetButton("Test");
		if(!useJoycons || test){
			return test;
		}

		return GetJoyconButton(testButtons);
	}

	public bool GetSpawnEnemyDown(){
		return Input.GetButtonDown("SpawnAI");
	}

	public bool GetClearTraceDown(){
		return Input.GetButtonDown("ClearTraces");
	}

	public bool GetDisplayDebugDown(){
		return Input.GetButtonDown("DisplayDebug");
	}

	public bool GetToggleJoyconsDown(){
		return Input.GetButtonDown("ToggleJoycons");
	}

	public bool GetAirControlButtonDown(){
		bool airControl = Input.GetButtonDown("AirControl");
		if(!useJoycons || airControl){
			return airControl;
		}

		return GetJoyconButtonDown(airControlButtons);
	}

	public bool GetRespawnButtonDown(){
		bool respawn = Input.GetButtonDown("Respawn");
		if(!useJoycons || respawn){
			return respawn;
		}

		return GetJoyconButtonDown(respawnButtons);
	}

	public bool GetClearAIsDown(){
		bool clear = Input.GetButtonDown("ClearAI");
		if(!useJoycons || clear){
			return clear;
		}

		return GetJoyconButtonDown(clearEnemyButtons);
	}

	public bool GetFakeAnimationDown(){
		return GetJoyconButtonDown(beginFakeJoyconsButtons);
	}

	public bool GetTimeUp(){
		return false; //Input.GetButtonDown("TimeUp");
	}

	public bool GetTimeDown(){
		return false; // Input.GetButtonDown("TimeDown");
	}

	public bool GetSensibilityUp(){
		return GetJoyconButtonDown(sensibilityUpButtons);
	}

	public bool GetSensibilityDown(){
		return GetJoyconButtonDown(sensibilityDownButtons);
	}

	#endregion

	public Color GetJoyconColor(JoyconType type){
		if((joyconColors != null) && joyconColors.ContainsKey(type)){
			return joyconColors[type];
		}
		else{
			return Color.black;
		}
	}

	private bool GetJoyconButton(JoyconInput[] inputArray){
		return false;//IT
	}

	private bool GetJoyconButtonDown(JoyconInput[] inputArray){
		return false; //IT
	}

	public Vector2 GetStickAxis(JoyconType type){
		if(!useJoycons){
			return Vector2.zero;
		}

		if(Joycons.Count != 2){
			return Vector2.zero;
		}

		float[] resultArray = Joycons[type].GetStick();
		Vector2 result = new Vector2(resultArray[0], resultArray[1]);

		return result;
	}

	public void ChangeMainHand(){
		ChangeMainHand(!useLeftHand);
	}

	public void ChangeMainHand(bool leftHand){
		if(leftHand){
			mainHand = JoyconType.LEFT;
			horizontalMultiply = -1f;
		}
		else{
			mainHand = JoyconType.RIGHT;
			horizontalMultiply = 1f;
		}

		useLeftHand = leftHand;
	}

	public int ChangeSensibilityLevel(int direction){
		actualLevel = Mathf.Max(Mathf.Min(actualLevel + direction, nbSensibilityLevel), 1);
		CalculateSensibility();
		return (int) actualLevel;
	}

	private void CalculateSensibility(){
		float nbLevel = nbSensibilityLevel;
		float level = (largestMaxDeadZone - smallestMaxDeadZone) / (nbLevel - 1f);
		actualDeadZone = smallestMaxDeadZone + level* (nbLevel - actualLevel);
	}
}
