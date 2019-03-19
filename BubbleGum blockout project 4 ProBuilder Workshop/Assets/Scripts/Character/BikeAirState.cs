using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(CollisionGetter))]
public class BikeAirState : MonoBehaviour {

	[Header("Raycast")]
	public Transform frontRaycast;
	public Transform backRaycast;
	[SerializeField] private float raycastLength = 0f;
	[SerializeField] private LayerMask raycastIgnoredLayers = 0;

	[Header("Do not touch")]
	[SerializeField] private bool canFall = false;

	[Header("Debug")]
	[SerializeField] private bool debugRay = false;

	public bool OnGround {get; private set;}
	public bool InFall {get; private set;}
	public RaycastHit BackGroundHit {get; private set;}
	public RaycastHit FrontGroundHit {get; private set;}

	private event Action OnGroundEvent;
	private event Action<NoBreakJump> NoBreakJumpEvent;
	private event Action InAirEvent;
	private event Action InFallEvent;

	private void Awake() {
		RegisterOnGround(SetOnGround);
		RegisterInAir(SetInAir);

		if(canFall){
			RegisterInFall(SetInFall);
			SetInFall();
		}
		else{
			SetInAir();
		}
	}

	private void Start() {
		GetComponent<CollisionGetter>()?.RegisterEnterCollision(OnCollision);
	}

	private void Update() {
		if((frontRaycast == null) || (backRaycast == null)){
			return;
		}

		RaycastHit hitBack;
		RaycastHit hitFront;
		bool backOnGround = Physics.Raycast(backRaycast.position, -Vector3.up, out hitBack, raycastLength, ~raycastIgnoredLayers);
		bool frontOnground = Physics.Raycast(frontRaycast.position, -Vector3.up, out hitFront, raycastLength, ~raycastIgnoredLayers);

		BackGroundHit = hitBack;
		FrontGroundHit = hitFront;
		
		bool onGround = backOnGround && frontOnground;

		if(!onGround && OnGround){
			InAirEvent.Invoke();
			return;
		}

		if(canFall){
			if(!OnGround && !InFall){
				return;
			}
			

			if(InFall && onGround){
				OnGroundEvent.Invoke();
			}
		}
		else{
			if(onGround && !OnGround){
				OnGroundEvent.Invoke();
			}
		}
		
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if(!debugRay){
			return;
		}
		
		if((frontRaycast == null) || (backRaycast == null)){
			return;
		}

		// Transform t = GetComponent<Transform>();

		Debug.DrawRay(frontRaycast.position, -Vector3.up * raycastLength, Color.red);
		Debug.DrawRay(backRaycast.position, -Vector3.up * raycastLength, Color.red);
	}
	#endif

	public void RegisterOnGround(Action action){
		OnGroundEvent += action;
	}

	public void UnRegisterOnGround(Action action){
		OnGroundEvent -= action;
	}

	public void RegisterNoBreakJump(Action<NoBreakJump> action){
		NoBreakJumpEvent += action;
	}

	public void UnRegisterNoBreakJump(Action<NoBreakJump> action){
		NoBreakJumpEvent -= action;
	}

	public void RegisterInAir(Action action){
		InAirEvent += action;
	}

	public void UnRegisterInAir(Action action){
		InAirEvent -= action;
	}

	public void RegisterInFall(Action action){
		InFallEvent += action;
	}

	public void UnRegisterInfall(Action action){
		InFallEvent -= action;
	}

	private void SetOnGround(){
		this.OnGround = true;
		this.InFall = false;		
	}

	private void SetInAir(){
		this.InFall = false;
		this.OnGround = false;
	}

	private void SetInFall(){
		this.InFall = true;
		this.OnGround = false;
	}

	private void OnCollision(Collision other){
		NoBreakJump component = other.gameObject.GetComponent<NoBreakJump>();

		if(component != null){
			NoBreakJumpEvent?.Invoke(component);
			return;
		}

		if(InFall || OnGround){
			return;
		}

		InFallEvent.Invoke();
	}
}
