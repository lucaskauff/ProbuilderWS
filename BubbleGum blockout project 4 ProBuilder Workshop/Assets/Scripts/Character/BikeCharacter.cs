using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[RequireComponent(typeof(BikeAnimation))]
[RequireComponent(typeof(BikeAirState), typeof(CollisionGetter))]
public class BikeCharacter : SerializedMonoBehaviour {

	[Header("Sticky")]
	[SerializeField] private int stickyLayer = 0;

	[Header("Gravity")]
	[SerializeField] private float airGravitySpeed = 1f;
	[SerializeField] private float groundGravitySpeed = 1f;

	[Header("Do not touch")]
	[SerializeField] private bool useXAxis = false;

	[Header("References")]
	public Transform parentTransform = null;

	public BikeComponents BikeComponents {get; private set;}

	protected virtual void Awake() {
		BikeComponents = new BikeComponents(gameObject);
	}

	protected void BikeRotation(float speed){
		Vector3 rotation = BikeComponents.Transform.rotation.eulerAngles;

		float value;
		Vector3 axis;
		Vector3 plane;

		if(useXAxis){
			value = rotation.x;
			axis = BikeComponents.Transform.right;
			plane = BikeComponents.Transform.forward;
		}
		else{
			value = rotation.z;
			axis = BikeComponents.Transform.forward;
			plane = BikeComponents.Transform.right;
		}

		if(value > 180){
			value -= 360;
		}

		plane.y = 0;		

		if(!BikeComponents.AirState.OnGround){
			float scaledGravitySpeed = airGravitySpeed * Time.deltaTime;
			if(Mathf.Abs(value) < scaledGravitySpeed){
				value = 0f;
			}
			else{
				value -= Mathf.Sign(value) * scaledGravitySpeed;
			}
		}
		else{
			RaycastHit frontHit = BikeComponents.AirState.FrontGroundHit;
			RaycastHit backHit = BikeComponents.AirState.BackGroundHit;

			float scaledGravitySpeed = groundGravitySpeed * Time.deltaTime;
			Vector3 gradient = frontHit.point - backHit.point;
			float angle = -Vector3.SignedAngle(gradient, plane, axis);
			
			if(frontHit.collider.gameObject.layer == stickyLayer){
				value = angle;
			}
			else{
				float diff = angle - value;
				float s = scaledGravitySpeed / (1 + speed);
				if((diff < 0) || (Mathf.Abs(diff) < s)){
					value = angle;
				}
				else{
					value += Mathf.Sign(diff) * s;
				}
				
			}
		}

		if(Mathf.Abs(value) > 90f){
			value = 90f * Mathf.Sign(value);
		}

		if(useXAxis){
			rotation.x = value;
		}
		else{
			rotation.z = value;
		}

		BikeComponents.Transform.rotation = Quaternion.Euler(rotation);
	}
}
