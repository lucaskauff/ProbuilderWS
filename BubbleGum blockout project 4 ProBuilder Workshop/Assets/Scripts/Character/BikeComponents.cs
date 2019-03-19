using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeComponents {

	public Transform Transform {get; private set;}
	public BikeAnimation Animations {get; private set;}
	public BikeAirState AirState {get; private set;}
	public CollisionGetter Collisions {get; private set;}

	public BikeComponents(GameObject parent){
		SetComponents(parent);
	}

	private void SetComponents(GameObject parent){
		Transform = parent.GetComponent<Transform>();
		Animations = parent.GetComponent<BikeAnimation>();
		Collisions = parent.GetComponent<CollisionGetter>();
		AirState = parent.GetComponent<BikeAirState>();
		// selfRigidbody = GetComponent<Rigidbody>();
	}

}
