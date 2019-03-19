using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

[RequireComponent(typeof(SoundPlayer))]
public class Bumper : MonoBehaviour, NoBreakJump, JumpScoring {

	[SerializeField] private float force = 10f;

	private Transform selfTransform;

	private SoundPlayer soundPlayer;

	private void Awake() {
		selfTransform = GetComponent<Transform>();
		soundPlayer = GetComponent<SoundPlayer>();
	}

	private void OnCollisionEnter(Collision other) {
		OnCollision(other.collider);
	}

	private void OnTriggerEnter(Collider other) {
		OnCollision(other);
	}

	private void OnCollision(Collider other){
		InteractableEntity otherEntity = other.GetComponent<InteractableEntity>();

		if(otherEntity == null){
			return;
		}

		Rigidbody rigidbody = other.GetComponent<Rigidbody>();
		rigidbody?.AddForce(selfTransform.up * force, ForceMode.VelocityChange);
	}
}
