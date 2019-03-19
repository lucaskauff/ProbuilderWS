using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BikeAnimation : MonoBehaviour {

	[Header("Wheel")]
	[SerializeField] private float wheelSpeed = 0f;
	[SerializeField] private float driveSpeed = 0f;

	[Header("Handlebar")]
	[SerializeField] private float barRotationSpeed = 0f;
	[SerializeField] private float barMaxRotationAngle = 0f;

	[Header("Bike")]
	[SerializeField] private float bikeMaxRotationAngle = 0f;

	[Header("Drift")]
	[Range(0f, 180f)]
	[SerializeField] private float driftVisualAngle = 0f;
	[SerializeField] private float driftRotationSpeed = 0f;

	[Header("Do not touch")]
	[SerializeField] private bool useXAxis = false;
	[SerializeField] private float axisSign = 0f;

	[Header("References")]
	[SerializeField] private Transform bike = null;
	[SerializeField] private Transform handlebars = null;
	[SerializeField] private Transform drive = null;
	[SerializeField] private Transform wheelFront = null;
	[SerializeField] private Transform wheelRear = null;
	[SerializeField] private Animator characterAnimator = null;
	[SerializeField] public Transform bikePivot = null;

	private float basicBarRotation;
	private float basicBikeRotation;
	private float basicPivotRotation;
	private float synchronizedSpeed;

	private void Awake() {
		basicBarRotation = RotationCorrection(handlebars.localRotation.eulerAngles.z);
		basicBikeRotation = RotationCorrection(bike.localRotation.eulerAngles.z);

		if(bikePivot != null){
			basicPivotRotation = RotationCorrection(bikePivot.localRotation.eulerAngles.y);
		}
		// synchronizedSpeed = 
	}

	public void UpdateAnimations(float speed, float maxSpeed, float horizontal, bool updateDrive = true, bool updateWheels = true, bool updateRotation = true){
		
		if(updateWheels){
			UpdateWheels(-speed);
		}

		if(updateDrive){
			UpdateDrive(-speed);
		}

		UpdateAnimationState(speed, maxSpeed);

		if(updateRotation){
			UpdateBar(horizontal);
			UpdateBike(horizontal);
		}
		
	}

	private void UpdateAnimationState(float speed, float maxSpeed){
		characterAnimator.SetFloat("Speed", speed / maxSpeed);
	}

	private void UpdateWheels(float speed){
		wheelFront.Rotate(new Vector3(speed * wheelSpeed * Time.deltaTime, 0f, 0f));
		wheelRear.Rotate(new Vector3(speed * wheelSpeed * Time.deltaTime, 0f, 0f));
	}

	private void UpdateDrive(float speed){
		drive.Rotate(new Vector3(speed * driveSpeed * Time.deltaTime, 0f, 0f));
	}

	private void UpdateBar(float horizontal){
		float targetRotation = RotationCorrection(Mathf.Lerp(basicBarRotation - barMaxRotationAngle, basicBarRotation + barMaxRotationAngle, (horizontal + 1f) / 2f));
		Vector3 actualRotation = handlebars.localRotation.eulerAngles;
		actualRotation.z = RotationCorrection(actualRotation.z);
		float scaledRotationSpeed = barRotationSpeed * Time.deltaTime;
		if(Mathf.Abs(targetRotation - actualRotation.z) < scaledRotationSpeed){
			actualRotation.z = targetRotation;
		}
		else{
			actualRotation.z += Mathf.Sign(targetRotation - actualRotation.z) * scaledRotationSpeed;
		}

		handlebars.localRotation = Quaternion.Euler(actualRotation);
	}

	private void UpdateBike(float horizontal){
		float targetRotation = RotationCorrection(Mathf.Lerp(basicBikeRotation - bikeMaxRotationAngle, basicBikeRotation + bikeMaxRotationAngle, (axisSign * horizontal + 1f) / 2f));
		Vector3 actualRotation = bike.localRotation.eulerAngles;
		
		float value;

		if(useXAxis){
			value = actualRotation.x;
		}
		else{
			value = actualRotation.z;
		}

		value = RotationCorrection(value);
		float scaledRotationSpeed = barRotationSpeed * Time.deltaTime;
		if(Mathf.Abs(targetRotation - value) < scaledRotationSpeed){
			value = targetRotation;
		}
		else{
			value += Mathf.Sign(targetRotation - value) * scaledRotationSpeed;
		}

		if(useXAxis){
			actualRotation.x = value;
		}
		else{
			actualRotation.z = value;
		}

		bike.localRotation = Quaternion.Euler(actualRotation);
	}

	public void UpdateDriftRotation(bool drift, float driftDirection){
		float target;
		float direction;
		if(drift){
			direction = driftDirection;
			target = driftDirection * driftVisualAngle;
		}
		else{
			direction = -driftDirection;
			target = basicPivotRotation;
		}

		float value = RotationCorrection(bikePivot.localRotation.eulerAngles.y);

		float diff = target - value;

		float speed = driftRotationSpeed * Time.deltaTime;


		if(Mathf.Abs(diff) < speed){
			value = target;
		}
		else{
			value += direction * speed;
		}
		
		bikePivot.localRotation = Quaternion.Euler(0f, value, 0f);
	}

	private float RotationCorrection(float rotation){
		if(rotation > 180){
			rotation -= 360;
		}

		return rotation;
	}
}
