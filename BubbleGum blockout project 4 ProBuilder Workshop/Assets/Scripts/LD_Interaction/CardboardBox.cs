using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CardboardBox : MonoBehaviour {

	private static readonly int flyingLayer = 13;
	// private static readonly int pushingLayer = 14;

	private Rigidbody selfRigidbody;

	[SerializeField] private float forceMultiply = 0f;
	[SerializeField] private float upForceMultiply = 0f;
	[SerializeField] private float maxSpeed = 0f;
	[SerializeField] private int score = 0;

	private void Awake() {
		selfRigidbody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "Player"){
			float speed = Mathf.Min(other.gameObject.GetComponent<PlayerCharacter>().Speed, maxSpeed);
			Vector3 force = (-other.impulse + (upForceMultiply * Vector3.up)) * speed * forceMultiply;
			selfRigidbody.AddForce(force, ForceMode.Acceleration);

			ScoringManager.Instance?.AddScore(score);
		}
	}

	private void Reset() {
		forceMultiply = 20f;
		upForceMultiply = 0f;
		gameObject.layer = flyingLayer;
	}
}
