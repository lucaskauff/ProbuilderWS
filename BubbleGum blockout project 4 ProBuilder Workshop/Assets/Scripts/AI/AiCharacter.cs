using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiCharacter : BikeCharacter {

	[Header("AI Informations")]
	[SerializeField] private float speedMultiplicator = 0f;
	[SerializeField] private float angularSpeedMultiplicator = 0f;

	[SerializeField] private NavMeshAgent agent = null;

	private Transform agentTransform;

	protected override void Awake() {
		base.Awake();
		agentTransform = agent.GetComponent<Transform>();
	}

	private void FixedUpdate() {
		Vector3 speed = agentTransform.InverseTransformDirection(agent.velocity).normalized;
		BikeRotation(speed.z);
		BikeComponents.Animations.UpdateAnimations(speedMultiplicator * speed.z, speedMultiplicator * speed.z, speed.x * angularSpeedMultiplicator);
	}
}
