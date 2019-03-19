using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomRumble : MonoBehaviour {

	[System.Serializable]
	public struct Rumble{
		public CustomInputManager.JoyconLink joycon;
		public float duration;
		public bool loop;
		public float priority;
		public float amplitudeMult;
		public AnimationCurve lowFreqCurve;
		public AnimationCurve highFreqCurve;
		public AnimationCurve amplitudeCurv;
		[HideInInspector] public float actualDuration;

		public void CopyValues(Rumble other, bool keepOldDuration = true){
			joycon = other.joycon;
			duration = other.duration;
			loop = other.loop;
			priority = other.priority;
			amplitudeMult = other.amplitudeMult;
			lowFreqCurve = other.lowFreqCurve;
			highFreqCurve = other.highFreqCurve;
			amplitudeCurv = other.amplitudeCurv;

			if(!keepOldDuration){
				actualDuration = other.actualDuration;
			}
		}
	}

	private CustomInputManager inputManager;
	private Dictionary<CustomInputManager.JoyconType, Joycon> joycons;

	private Dictionary<int, Rumble> actualVibrations;

	private List<int> removedRumbles;

	private int actualId;

	private void Awake() {
		inputManager = CustomInputManager.Instance;
		joycons = inputManager.Joycons;

		actualVibrations = new Dictionary<int, Rumble>();
		removedRumbles = new List<int>();
	}

	private void Update() {
		if(actualVibrations.Count < 0){
			return;
		}

		bool hasLeftRumble = false;
		bool hasRightRumble = false;
		Rumble actualLeftRumble = new Rumble();
		Rumble actualRightRumble = new Rumble();

		foreach (int rumbleId in actualVibrations.Keys.ToArray())
		{
			Rumble r = actualVibrations[rumbleId];
			r.actualDuration += Time.deltaTime;

			if(r.actualDuration > r.duration){
				if(r.loop){
					r.actualDuration -= r.duration;
				}
				else{
					removedRumbles.Add(rumbleId);
					continue;
				}
			}

			actualVibrations[rumbleId] = r;

			if(IsSameJoycon(r, CustomInputManager.JoyconLink.LEFT)){
				if(!hasLeftRumble){
					actualLeftRumble = r;
					hasLeftRumble = true;
				}
				else if(r.priority > actualLeftRumble.priority){
					actualLeftRumble = r;
				}
			}

			if(IsSameJoycon(r, CustomInputManager.JoyconLink.RIGHT)){
				if(!hasRightRumble){
					actualRightRumble = r;
					hasRightRumble = true;
				}
				else if(r.priority > actualRightRumble.priority){
					actualRightRumble = r;
				}
			}
			
		}

		if(inputManager.useJoycons && (joycons.Count == 2)){
			if(hasLeftRumble){
				ApplyRumbles(actualLeftRumble, CustomInputManager.JoyconType.LEFT);
			}
			else{
				joycons[CustomInputManager.JoyconType.LEFT].SetRumble(0f, 0f, 0f);
			}

			if(hasRightRumble){
				ApplyRumbles(actualRightRumble, CustomInputManager.JoyconType.RIGHT);
			}
			else{
				joycons[CustomInputManager.JoyconType.RIGHT].SetRumble(0f, 0f, 0f);
			}
		}

		foreach (int removed in removedRumbles)
		{
			actualVibrations.Remove(removed);
		}

		removedRumbles.Clear();
		
		// joycons[CustomInputManager.JoyconType.LEFT].SetRumble()
	}

	public void ApplyRumbles(Rumble rumble, CustomInputManager.JoyconType joycon){
		float time = rumble.actualDuration / rumble.duration;
		float low = rumble.lowFreqCurve.Evaluate(time);
		float high = rumble.highFreqCurve.Evaluate(time);
		float ampl = rumble.amplitudeCurv.Evaluate(time);

		joycons[joycon].SetRumble(low, high, /*Mathf.Clamp(*/ampl * rumble.amplitudeMult/*, 0f, 1f)*/);
	}

	public int BeginRumble(Rumble vibration){
		while(actualVibrations.ContainsKey(actualId)){
			actualId++;

			if(actualId > 1000){
				actualId = 0;
			}
		}

		actualVibrations.Add(actualId, vibration);
		return actualId;
	}

	public void ModifyRumble(int rumbleId, Rumble modifiedRumble, bool keepOldDuration = true){
		if(!actualVibrations.ContainsKey(rumbleId)){
			return;
		}

		Rumble rumble = actualVibrations[rumbleId];
		rumble.CopyValues(modifiedRumble, keepOldDuration);
		actualVibrations[rumbleId] = rumble;
	}

	public void EndRumble(int rumbleId){
		if(!actualVibrations.ContainsKey(rumbleId)){
			return;
		}

		actualVibrations.Remove(rumbleId);
	}

	public bool IsSameJoycon(Rumble rumble, CustomInputManager.JoyconLink type){
		return (rumble.joycon == CustomInputManager.JoyconLink.BOTH) || (rumble.joycon == type);
	}

	private void OnDestroy() {
		//foreach (CustomInputManager.JoyconType j in joycons.Keys){
		//	joycons[j].SetRumble(0f, 0f, 0f);
		//}
	}
}
