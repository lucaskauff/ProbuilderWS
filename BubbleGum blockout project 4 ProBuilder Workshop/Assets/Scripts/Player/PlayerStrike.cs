using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStrike : MonoBehaviour {

	[SerializeField] private bool canBeGrab = true;
	[SerializeField] private float boostAfterGrab = 0f;
	[SerializeField] private float imunityTime = 0f;
	[SerializeField] private float completeGrabTime = 0f;
	[SerializeField] private float qteTime = 0f;

	[Header("References")]
	[SerializeField] private CanvasGroup grabGroup = null;
	[SerializeField] private CanvasGroup qteGroup = null;
	[SerializeField] private TMP_Text grabTimeText = null;

	public event Action OnGrabEvent;

	public bool InGrab {get; private set;}

	private PlayerCharacter character;

	private float grabTiming;

	public bool InImmunity {get; private set;}

	private void Awake() {
		character = GetComponent<PlayerCharacter>();
		InImmunity = false;
	}

	private void Start() {
		character.BikeComponents.Collisions.RegisterEnterCollision(OnCollision);
	}

	private void Update() {
		if(InImmunity){
			grabTiming += Time.deltaTime;
			if(grabTiming > imunityTime){
				InImmunity = false;
			}
		}

		if(InGrab){
			grabTiming += Time.deltaTime;
			grabTimeText.text = ((int) (completeGrabTime - grabTiming)).ToString();
			if(grabTiming > completeGrabTime){
				Ungrab();
			}
			else if(grabTiming > qteTime){
				qteGroup.alpha = 1f;
				if(CustomInputManager.Instance.GetUngrabDown()){
					Ungrab();
				}
			}
		}
	}

	private void OnCollision(Collision other){
		if(!canBeGrab){
			return;
		}

		if(!InImmunity && !InGrab && (other.gameObject.tag == "AI")){
			character.StopBike();
			InGrab = true;
			CustomInputManager.Instance.Lock(LockInputType.MOVEMENT, this);
			grabTiming = 0f;
			grabTimeText.text = ((int) completeGrabTime).ToString();
			grabGroup.alpha = 1f;

			OnGrabEvent?.Invoke();
		}
	}

	private void Ungrab(){
		CustomInputManager.Instance.UnLock(LockInputType.MOVEMENT, this);
		InGrab = false;
		character.BoostSpeed(boostAfterGrab);
		qteGroup.alpha = 0f;
		grabGroup.alpha = 0f;
		grabTiming = 0f;
		InImmunity = true;
	}
}
