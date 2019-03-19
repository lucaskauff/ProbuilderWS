using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour {

	[SerializeField] private float turningSpeed = 0f;
	[SerializeField] private float boostAfter = 0f;

	private Transform selfTransform;
	private PlayerCharacter character;
	private Quaternion basicRotation;

	private bool isTurning;
	private float rotationSign;
	private CustomInputManager.InputAsk joyconSide;

	private void Awake() {
		selfTransform = GetComponent<Transform>();
		character = FindObjectOfType<PlayerCharacter>();
		basicRotation = selfTransform.rotation;
		isTurning = false;
	}

	private void Start() {
		character.PlayerComponents.Hands.leftBox.RegisterEnterTrigger(LeftHandTriggerEnter);
		character.PlayerComponents.Hands.rightBox.RegisterEnterTrigger(RightHandTriggerEnter);
	}

	private void FixedUpdate() {
		if(!isTurning){
			return;
		}

		CustomInputManager.InputAsk side = CustomInputManager.Instance.GetHandUp();

		if((side != joyconSide) && (side != CustomInputManager.InputAsk.BOTH)){
			EndTurnAround();
			return;
		}

		Vector3 rotation = selfTransform.rotation.eulerAngles;
		rotation.y += rotationSign * turningSpeed * Time.deltaTime;
		selfTransform.rotation = Quaternion.Euler(rotation);
	}

	private void LeftHandTriggerEnter(Collider other){
		if(other.gameObject != gameObject){
			return;
		}

		joyconSide = CustomInputManager.InputAsk.LEFT;
		rotationSign = -1f;
		BeginTurnAround();
	}

	private void RightHandTriggerEnter(Collider other){
		if(other.gameObject != gameObject){
			return;
		}

		joyconSide = CustomInputManager.InputAsk.RIGHT;
		rotationSign = 1f;
		BeginTurnAround();
	}

	private void BeginTurnAround(){
		isTurning = true;
		CustomInputManager.Instance.Lock(LockInputType.MOVEMENT, this);
		character.StopBike();
		character.PlayerComponents.Rigidbody.isKinematic = true;
		character.parentTransform.parent = selfTransform;
	}

	private void EndTurnAround(){
		isTurning = false;
		character.parentTransform.parent = null;
		CustomInputManager.Instance.UnLock(LockInputType.MOVEMENT, this);
		selfTransform.rotation = basicRotation;
		character.PlayerComponents.Rigidbody.isKinematic = false;
		character.BoostSpeed(boostAfter);
	}

	private void Reset() {
		gameObject.layer = PlayerHands.handLayer;
	}
}
