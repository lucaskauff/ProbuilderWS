using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionGetter : MonoBehaviour {

	private event Action<Collision> enterCollisionEvent;
	private event Action<Collision> exitCollisionEvent;
	private event Action<Collider> enterTriggerEvent;
	private event Action<Collider> exitTriggerEvent;

	public void RegisterEnterCollision(Action<Collision> action){
		enterCollisionEvent += action;
	}

	public void RegisterExitCollision(Action<Collision> action){
		exitCollisionEvent += action;
	}

	public void RegisterEnterTrigger(Action<Collider> action){
		enterTriggerEvent += action;
	}

	public void RegisterExitTrigger(Action<Collider> action){
		exitTriggerEvent += action;
	}

	public void UnRegisterEnterCollision(Action<Collision> action){
		enterCollisionEvent -= action;
	}

	public void UnRegisterExitCollision(Action<Collision> action){
		exitCollisionEvent -= action;
	}

	public void UnRegisterEnterTrigger(Action<Collider> action){
		enterTriggerEvent -= action;
	}

	public void UnRegisterExitTrigger(Action<Collider> action){
		exitTriggerEvent -= action;
	}

	private void OnCollisionEnter(Collision other) {
		enterCollisionEvent?.Invoke(other);
	}

	private void OnTriggerEnter(Collider other) {
		enterTriggerEvent?.Invoke(other);
	}

	private void OnCollisionExit(Collision other) {
		exitCollisionEvent?.Invoke(other);
	}

	private void OnTriggerExit(Collider other) {
		exitTriggerEvent?.Invoke(other);
	}
}
