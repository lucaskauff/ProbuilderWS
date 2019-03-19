using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeedback : MonoBehaviour {

	[SerializeField] private bool useContinuousRumble = false;
	[SerializeField] private bool useCollisionRumble = false;
	[SerializeField] private float minAmplRumble = 0.025f;

	[Header("Rumbles")]
	[SerializeField] private CustomRumble.Rumble continuousRumble = new CustomRumble.Rumble();
	[SerializeField] private CustomRumble.Rumble collisionRumble = new CustomRumble.Rumble();
	[SerializeField] private CustomRumble.Rumble boostRumble = new CustomRumble.Rumble();
	[SerializeField] private CustomRumble.Rumble driftRumble = new CustomRumble.Rumble();

	private PlayerCharacter playerCharacter;

	int id;

	private bool inRumble;

	private void Awake() {
		inRumble = false;
	}

	private void Start() {
		playerCharacter = GetComponent<PlayerCharacter>();
		playerCharacter.BikeComponents.Collisions.RegisterEnterCollision(OnCollisionEvent);
	}

	private void Update() {
		if(!useContinuousRumble){
			return;
		}

		CustomRumble.Rumble actualRumble = continuousRumble;


		switch(playerCharacter.ActualState){
			case PlayerCharacter.State.AIR:
			case PlayerCharacter.State.AIR_BOOST:
			case PlayerCharacter.State.AIR_TRICK:
				if(inRumble){
					StopRumble();
				}
				return;
			case PlayerCharacter.State.STOP:
				if(inRumble){
					StopRumble();
				}
				return;

			case PlayerCharacter.State.BOOST:
				actualRumble = boostRumble;
				break;

			case PlayerCharacter.State.DRIFT:
				actualRumble = driftRumble;
				break;
		}

		MakeRumble(actualRumble);
	}

	private CustomRumble.Rumble GetScaledRumble(CustomRumble.Rumble customRumble){
		float mult = (playerCharacter.Speed / playerCharacter.GetMaxSpeed());
		customRumble.duration *= 1f / mult;
		customRumble.amplitudeMult *= mult;

		if((customRumble.amplitudeCurv.Evaluate(customRumble.duration) * customRumble.amplitudeMult) < minAmplRumble){
			customRumble.amplitudeMult = 0f;
		}
		return customRumble;
	} 

	private void MakeRumble(CustomRumble.Rumble customRumble){
		CustomRumble.Rumble scaledRumble = GetScaledRumble(customRumble);
		if(!inRumble){
			id = CustomInputManager.Instance.InputVibrations.BeginRumble(scaledRumble);
			inRumble = true;
		}
		else{
			CustomInputManager.Instance.InputVibrations.ModifyRumble(id, scaledRumble);
		}
	}

	private void StopRumble(){
		CustomInputManager.Instance.InputVibrations.EndRumble(id);
		inRumble = false;
	}

	private void OnCollisionEvent(Collision other){
		if(!useCollisionRumble){
			return;
		}

		CustomRumble.Rumble customRumble = collisionRumble;
		customRumble.amplitudeMult *= other.impulse.magnitude;
		CustomInputManager.Instance.InputVibrations.BeginRumble(customRumble);
	}
}
