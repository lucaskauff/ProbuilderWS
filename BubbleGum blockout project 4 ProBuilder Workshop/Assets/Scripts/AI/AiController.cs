using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiController : MonoBehaviour {

	public enum State{WAIT, TO, RUSH}

	[SerializeField] private float followSpeed = 0f;
	[SerializeField] private float followDistance = 0f;
	[SerializeField] private float boostDistance = 0f;
	[SerializeField] private float boostSpeed = 0f;
	[SerializeField] private Transform defaultTarget = null;

	[Header("Conditions")]
	[SerializeField] private bool useBoost = false;

	[Header("Rush (Outdated)")]
	[SerializeField] private bool rush = false;
	[SerializeField] private float rushDistance = 0f;
	[SerializeField] private float rushTime = 0f;
	[SerializeField] private float rushSpeed = 0f;

	private float timer;

	private State actualState;

	public Transform SelfTransform {get; private set;}
	// private Rigidbody selfRigidbody;
	public NavMeshAgent NavMeshAgent {get; private set;}

	private Transform target;
	private Transform player;
	private PlayerCharacter playerCharacter;

	private void Awake() {
		SelfTransform = GetComponent<Transform>();
		// selfRigidbody = GetComponent<Rigidbody>();
		NavMeshAgent = GetComponent<NavMeshAgent>();
		playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
		player = playerCharacter.bikeCenter;
		target = null;
		SwitchState(State.TO);
	}

	private void Update() {
		SetDefaultTarget();

		if(playerCharacter.PlayerComponents.Strike.InGrab){
			return;
		}

		timer += Time.deltaTime;

		switch(actualState){
			case State.WAIT:
				if(Vector3.Distance(target.position, SelfTransform.position) < followDistance){
					SwitchState(State.TO);
				}
				break;
			case State.TO:
				if(rush && Vector3.Distance(target.position, SelfTransform.position) < rushDistance){
					SwitchState(State.RUSH);
				}
				
				if(NavMeshAgent.velocity == (Vector3.zero)){
					// LookPlayer();
				}
				
				break;
			case State.RUSH:
				if(timer > rushTime){
					SwitchState(State.TO);
				}
				break;
		}
	}

	private void FixedUpdate() {
		SetDefaultTarget();

		if(playerCharacter.PlayerComponents.Strike.InGrab){
			NavMeshAgent.isStopped = true;
			return;
		}

		NavMeshAgent.isStopped = false;

		switch(actualState){
			case State.TO:
				float speed;
				if(useBoost){
					if((target.position - SelfTransform.position).magnitude < boostDistance){
						speed = followSpeed;
					}
					else{
						speed = boostSpeed;
					}
				}
				else{
					speed = followSpeed;
				}
				
				NavMeshAgent.speed = speed;
				NavMeshAgent.destination = target.position;

				break;
			case State.RUSH:
				NavMeshAgent.speed = rushSpeed;
				NavMeshAgent.destination = SelfTransform.position + SelfTransform.forward;
				break;
		}
	}

	public void SetDefaultTarget(){
		if(target != null){
			return;
		}

		if(defaultTarget == null){
			target = player;
		}
		else{
			target = defaultTarget;
		}
	}

	public void SetTarget(Transform target, bool wait = false){
		this.target = target;

		if(wait){
			actualState = State.WAIT;
		}
		else{
			actualState = State.TO;
		}
	}

	private void LookPlayer(){
		SelfTransform.LookAt(player.position);
		// selfRigidbody.MoveRotation(Quaternion.Euler(new Vector3(0f, selfTransform.rotation.eulerAngles.y, 0f)));
		SelfTransform.rotation = Quaternion.Euler(new Vector3(0f, SelfTransform.rotation.eulerAngles.y, 0f));
	}

	private void SwitchState(State newState){
		actualState = newState;
		timer = 0f;
	}
}
