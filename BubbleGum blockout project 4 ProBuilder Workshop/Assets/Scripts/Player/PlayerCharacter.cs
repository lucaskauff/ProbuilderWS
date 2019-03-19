using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[RequireComponent(typeof(PlayerStrike), typeof(PlayerHands))]
[RequireComponent(typeof(PlayerFeedback), typeof(Rigidbody))]
[RequireComponent(typeof(PlayerTricks))]
// [RequireComponent(typeof())]
public class PlayerCharacter : BikeCharacter, InteractableEntity, OpeningDoor {

	public enum State{STOP, CONTINUOUS, BASIC, BRAKE, DRIFT, AIR, AIR_TRICK, BOOST, AFTER_BOOST, AIR_BOOST}

	[Header("Movements Scriptable Objects")]
	[OdinSerialize] private Dictionary<State, BikeMovement> movements = null;


	[Header("Drift")]
	[Range(0f, 1f)]
	[SerializeField] private float minDriftRotation = 0f;

	[Header("Camera Parameters")]
	[SerializeField, Range(0f, 1f)] private float turningCameraThreshold = 0f;
	[SerializeField] private float frontCameraThreshold = 0f;

	[Header("AirControl")]
	public bool airControl = true;

	[Header("References")]
	[SerializeField] private CameraManager cameraManager = null;
	[SerializeField] public Transform bikeCenter = null;

	[Header("Components")]
	public PlayerComponents PlayerComponents = new PlayerComponents();

	[Header("Debug")]
	[SerializeField] private int ldLayer = 9;

	public event Action ChangeStateEvent;

	private CustomInputManager inputManager;

	public float Speed {get; private set;}

	private float actualAngle;

	private bool verticalInput;
	private bool brakeInput;
	private float horizontalInput;
	public State ActualState {get; private set;}

	private bool onDrift;
	private float driftDirection;

	protected override void Awake() {
		base.Awake();
		PlayerComponents.SetComponents(gameObject);

		inputManager = CustomInputManager.Instance;
		if(inputManager == null){
			Debug.LogError("Need Input Manager Prefab in Scene");
		}

		Speed = 0f;
		BikeComponents.Collisions.RegisterEnterCollision(OnCollision);
		BikeComponents.Collisions.RegisterEnterTrigger(OnCollision);
	}

	private void Update() {
		verticalInput = inputManager.GetForwardButton();
		brakeInput = inputManager.GetBrakeButton();

		horizontalInput = inputManager.GetHorizontal();

		State newState = GetActualState(verticalInput, brakeInput);
		if(ActualState != newState){
			ActualState = newState;
			ChangeStateEvent?.Invoke();
		}

		bool canDrift = verticalInput && brakeInput;
		canDrift = canDrift && ((horizontalInput != 0f) || onDrift);
		canDrift = canDrift && (BikeComponents.AirState.OnGround || onDrift);

		if(canDrift){
			if(!onDrift){
				onDrift = true;
				driftDirection = Mathf.Sign(horizontalInput);
			}

			horizontalInput = ((horizontalInput + driftDirection) / (2f - (2f * minDriftRotation))) + driftDirection * minDriftRotation;
		}
		else if(onDrift){
			onDrift = false;
		}

		SwitchCamera(verticalInput, brakeInput, horizontalInput);
		
	}

	private void FixedUpdate() {
		BikeRotation(Speed);
		Move(verticalInput, brakeInput, horizontalInput);

		bool hasControl = (!inputManager.IsLocked(LockInputType.MOVEMENT) && (BikeComponents.AirState.OnGround || airControl));

		bool updateRotation = (hasControl && movements[ActualState].canTurn);

		bool updateDrive = BikeComponents.AirState.OnGround && !onDrift;

		BikeComponents.Animations.UpdateAnimations(Speed, GetMaxSpeed(), horizontalInput, updateDrive, !onDrift, updateRotation);
		BikeComponents.Animations.UpdateDriftRotation(onDrift, driftDirection);
	}

	private void SwitchCamera(bool forward, bool brake, float horizontal){
		CameraManager.CameraType type;

		if(inputManager.GetTestBool()){
			type = CameraManager.CameraType.BACK;
		}
		else if(inputManager.IsLocked(LockInputType.MOVEMENT)){
			type = CameraManager.CameraType.STOP;
		}
		else if(!forward){
			type = CameraManager.CameraType.STOP;
		}
		else{
			if(brake && BikeComponents.AirState.OnGround && (horizontal != 0f)){
				if(horizontal < 0){
					type = CameraManager.CameraType.LEFT_DRIFT;
				}
				else{
					type = CameraManager.CameraType.RIGHT_DRIFT;
				}
				
			}
			else if(inputManager.GetBoost()){
				type = CameraManager.CameraType.BOOST;
			}
			else if(!BikeComponents.AirState.OnGround){
				type = CameraManager.CameraType.AIR;
			}
			else if(Speed < frontCameraThreshold){
				type = CameraManager.CameraType.STOP;
			}
			else if(Mathf.Abs(horizontal) < turningCameraThreshold){
				type = CameraManager.CameraType.FRONT;
			}
			else if(horizontal < 0){
				type = CameraManager.CameraType.LEFT;
			}
			else{
				type = CameraManager.CameraType.RIGHT;
			}
		}

		cameraManager.SwitchCamera(type);
	}

	private void Move(bool vertical, bool brake, float horizontal){
		// selfRigidbody.velocity += selfTransform.forward * bikeSpeed * vertical * Time.deltaTime;
		bool hasControl = (!inputManager.IsLocked(LockInputType.MOVEMENT) && (BikeComponents.AirState.OnGround || airControl));
		bool boost = inputManager.GetBoost();

		float rSpeed = 0f;
		BikeMovement movement = movements[State.CONTINUOUS];

		if(hasControl){
			movement = movements[ActualState];
			if(movement.canTurn){
				rSpeed = movement.turnSpeed;
			}

			Speed = CalculateSpeed(movement);
		}

		// if(vertical && hasControl){
		// 	if(brake && BikeComponents.AirState.OnGround){
		// 		Speed = Mathf.Min(Mathf.Max(Speed - driftDeceleration * Time.deltaTime, 0f), maxDriftSpeed);
		// 	}
		// 	else if(boost){
		// 		Speed = Mathf.Min(Speed + boostAcceleration * Time.deltaTime, maxBoostSpeed);
		// 	}
		// 	else if(Speed > maxSpeed){
		// 		Speed = Mathf.Max(Speed - afterBoostDeceleration * Time.deltaTime, 0f);
		// 	}
		// 	else{
		// 		Speed = Mathf.Min(Speed + acceleration * Time.deltaTime, maxSpeed);
		// 	}
		// }
		// else if(!vertical && brake && hasControl){
		// 	Speed = Mathf.Max(Speed - brakeDeceleration * Time.deltaTime, 0f);
		// }
		// else{
		// 	Speed = Mathf.Max(Speed - continueDeceleration * Time.deltaTime, 0f);
		// }

		// if(hasControl){
		// 	float rSpeed = rotationSpeed;

		// 	if(PlayerComponents.Tricks.InTricks()){
		// 		rSpeed = 0f;
		// 	}
		// 	else if(vertical && brake && BikeComponents.AirState.OnGround){
		// 		rSpeed = driftRotationSpeed;
		// 	}
		// 	else if(boost && vertical){
		// 		if(turnInBoost){
		// 			rSpeed = boostRotationSpeed;
		// 		}
		// 		else{
		// 			rSpeed = 0f;
		// 		}
		// 	}
		// 	else if(!BikeComponents.AirState.OnGround){
		// 		rSpeed = airRotationSpeed;
		// 	}
			
		// 	BikeComponents.Transform.Rotate(new Vector3(0f, rSpeed * horizontal * Time.deltaTime, 0f), Space.World);
		// }
		
		BikeComponents.Transform.Rotate(new Vector3(0f, rSpeed * horizontal * Time.deltaTime, 0f), Space.World);
		BikeComponents.Transform.position += BikeComponents.Transform.forward * Speed * Time.deltaTime;
	}

	public State GetActualState(){
		return GetActualState(verticalInput, brakeInput);
	}

	private State GetActualState(bool vertical, bool brake){
		bool boost = inputManager.GetBoost();

		State state = State.CONTINUOUS;

		if(BikeComponents.AirState.OnGround){
			if(!vertical && (Speed == 0f)){
				state = State.STOP;
			}
			else if(vertical){
				if(brake){
					state = State.DRIFT;
				}
				else if(boost){
					state = State.BOOST;
				}
				else if(Speed > movements[State.BOOST].maxSpeed){
					state = State.AFTER_BOOST;
				}
				else{
					state = State.BASIC;
				}
			}
			else if(brake){
				state = State.BRAKE;
			}
		}
		else{
			if(PlayerComponents.Tricks.InTricks()){
				state = State.AIR_TRICK;
			}
			else if(boost){
				state = State.AIR_BOOST;
			}
			else{
				state = State.AIR;
			}
		}

		return state;
	}

	private float CalculateSpeed(BikeMovement movement){
		return Mathf.Min(Mathf.Max(Speed + movement.acceleration * Time.deltaTime, 0f), movement.maxSpeed);
	}

	private void OnCollision(Collision other) {
		OnCollision(other.collider);
	}

	private void OnCollision(Collider other){
		if(other.gameObject.layer == ldLayer){
			Renderer renderer = other.GetComponent<Renderer>();
			if(renderer != null){
				Material newMaterial = new Material(renderer.material);
				newMaterial?.SetColor("_EmissionColor", newMaterial.color);
				renderer.material = newMaterial;
			}
		}
	}

	public void ChangeAirControl(){
		ChangeAirControl(!airControl);
	}

	public void ChangeAirControl(bool newControl){
		airControl = newControl;
	}

	public void StopBike(){
		Speed = 0f;
	}

	public void BoostSpeed(float boostValue, bool overrideValue = false){
		if(overrideValue){
			Speed = boostValue;
		}
		else{
			Speed += boostValue;
		}
	}

	public float GetMaxSpeed(){
		return movements[State.BOOST].maxSpeed;
	}
}
