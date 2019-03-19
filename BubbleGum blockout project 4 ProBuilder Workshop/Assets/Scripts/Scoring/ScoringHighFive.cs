using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScoringManager))]
public class ScoringHighFive : MonoBehaviour {

	[Tooltip("Delay after the last high five before stop combo")]
	[SerializeField] private float highFiveDelay = 0f;

	private ScoringManager scoringManager;

	private float delay;

	private bool inHighFive;

	private int nbFive;

	private void Awake() {
		scoringManager = GetComponent<ScoringManager>();
		nbFive = 0;
		inHighFive = false;
	}

	private void Start() {
		scoringManager.Player.PlayerComponents.Hands.leftBox.RegisterEnterTrigger(HighFiveTrigger);
		scoringManager.Player.PlayerComponents.Hands.rightBox.RegisterEnterTrigger(HighFiveTrigger);

		// scoringManager.Player.SelfComponents.Hands.leftBox.RegisterExitTrigger(LeftHandTriggerExit);
		// scoringManager.Player.SelfComponents.Hands.rightBox.RegisterExitTrigger(RightHandTriggerExit);
	}

	private void Update() {
		if(!inHighFive){
			return;
		}

		delay += Time.deltaTime;

		if(delay > highFiveDelay){
			inHighFive = false;
			scoringManager.ScoringFeedback.StopHighFive();
			nbFive = 0;
		}
	}

	public void HighFiveTrigger(Collider other){
		HighFive component = other.GetComponent<HighFive>();
		if(component == null){
			return;
		}

		nbFive++;
		scoringManager.ScoringFeedback.SetHighFive(nbFive);
		delay = 0f;
		inHighFive = true;
		component.MakeFive();
	}

	// public void LeftHandTriggerEnter(Collider other){
	// 	Debug.Log("Left Enter");
	// }

	// public void RightHandTriggerEnter(Collider other){
	// 	Debug.Log("Right Enter");
	// }

	// public void LeftHandTriggerExit(Collider other){
	// 	Debug.Log("Left Exit");
	// }

	// public void RightHandTriggerExit(Collider other){
	// 	Debug.Log("Right Exit");		
	// }
}
