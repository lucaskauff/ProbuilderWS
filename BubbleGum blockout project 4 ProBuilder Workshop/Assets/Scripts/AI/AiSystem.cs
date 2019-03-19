using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AiFollowSystem), typeof(AiPulling))]
public class AiSystem : MonoBehaviour {

	public static AiSystem Instance {get; private set;}

	[SerializeField] private float despawnDistance = 0f;
	[SerializeField] private Transform spawnedAIParent = null;

	public PlayerCharacter Player {get; private set;}
	public event Action ClearAIEvent;

	private List<AiController> spawnedEnemies;

	// private AiFollowSystem followSystem;
	private AiPulling aiPulling;


	private void Awake() {
		if(Instance != null){
			if(Instance != this){
				Destroy(gameObject);
			}

			return;
		}

		Instance = this;

		spawnedEnemies = new List<AiController>();
		Player = FindObjectOfType<PlayerCharacter>();
		// followSystem = GetComponent<AiFollowSystem>();
		aiPulling = GetComponent<AiPulling>();
	}

	private void Update() {
		foreach(AiController ai in spawnedEnemies.ToArray()){
			if(Vector3.Distance(ai.SelfTransform.position, Player.BikeComponents.Transform.position) > despawnDistance){
				ClearAI(ai);
			}
		}
	}

	public void ClearAIs(){
		foreach(AiController ai in spawnedEnemies.ToArray()){
			aiPulling.ReleaseAI(ai);
		}

		spawnedEnemies.Clear();
		ClearAIEvent?.Invoke();
	}

	public void ClearAI(AiController ai){
		aiPulling.ReleaseAI(ai);

		if(spawnedEnemies.Contains(ai)){
			spawnedEnemies.Remove(ai);
		}
	}

	public AiController SpawnAI(Vector3 position, bool wait = false){
		return SpawnAI(position, Player.bikeCenter, wait);
	}

	public AiController SpawnAI(Vector3 position, Transform target, bool wait = false){
		AiController newAi = aiPulling.AskNewAI(spawnedAIParent, position, target, wait);
		spawnedEnemies.Add(newAi);

		return newAi;
	}
}
