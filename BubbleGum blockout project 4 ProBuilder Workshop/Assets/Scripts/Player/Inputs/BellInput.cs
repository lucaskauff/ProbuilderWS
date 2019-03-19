using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellInput : MonoBehaviour {

	[Tooltip("Time to count bell input")]
	[SerializeField] private float bellPermissiveTime = 0f;
	[SerializeField] private float minStickUpValue = 0f;
	[SerializeField] private float maxStickDownValue = 0f;

	public bool BellDown {get; private set;}

	private float actualTime;

	private bool bellUp;

	private CustomInputManager inputManager;

	private void Awake() {
		BellDown = false;
		bellUp = false;
		inputManager = CustomInputManager.Instance;
	}
	
	void Update () {
		if(BellDown){
			BellDown = false;
			return;
		}

		if(inputManager.GetStickAxis(CustomInputManager.JoyconType.LEFT).magnitude >= minStickUpValue){
			bellUp = true;
			actualTime = 0f;
			return;
		}

		if(bellUp){
			if(inputManager.GetStickAxis(CustomInputManager.JoyconType.LEFT).magnitude <= maxStickDownValue){
				BellDown = true;
				bellUp = false;
			}
			else{
				actualTime += Time.deltaTime;
				if(actualTime > bellPermissiveTime){
					bellUp = false;
				}
			}
		}
		
	}
}
