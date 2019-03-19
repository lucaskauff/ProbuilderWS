using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class BikeBell : MonoBehaviour {

	[SerializeField] private Sound sound = null;

	private CustomInputManager inputManager;

	private void Awake() {
		inputManager = CustomInputManager.Instance;
		sound.Init(FindObjectOfType<PlayerCharacter>().BikeComponents.Transform);
	}

	private void Update() {
		if(inputManager.GetBellDown()){
			sound.PlaySound();
		}
	}
}
