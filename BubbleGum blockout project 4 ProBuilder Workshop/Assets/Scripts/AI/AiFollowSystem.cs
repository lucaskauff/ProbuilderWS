using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollowSystem : MonoBehaviour {

	[SerializeField] private bool useFollow = false;

	[SerializeField] private int maxAI = 0;

	[SerializeField] private float timeBetweenAI = 0f;

	[Header("Spawn infos")]
	[SerializeField] private float minSpawnDistance = 0f;
	[SerializeField] private float maxSpawnDistance = 0f;
	[SerializeField] private LayerMask ignoredLayers = 0;

	private AiSystem aiSystem;

	private PlayerCharacter player;

	private List<AiController> ais;

	private float time;

	private void Awake() {
		aiSystem = AiSystem.Instance;
		player = aiSystem.Player;
		aiSystem.ClearAIEvent += ClearAIEvent;
		ais = new List<AiController>();
		time = 0f;
	}

	private void Update() {
		if(!useFollow || (ais.Count >= maxAI)){
			return;
		}

		time += Time.deltaTime;

		if(((int) (time / timeBetweenAI)) > ais.Count){
			bool spawn = true;
			Vector3 position;
			RaycastHit hitInfo;
			if(Physics.Raycast(player.bikeCenter.position, -player.BikeComponents.Transform.forward, out hitInfo, maxSpawnDistance, ~ignoredLayers)){
				position = hitInfo.point + player.BikeComponents.Transform.forward;
				if((position - player.bikeCenter.position).magnitude < minSpawnDistance){
					spawn = false;
				}
			}
			else{
				position = player.bikeCenter.position - player.BikeComponents.Transform.forward * maxSpawnDistance;
			}

			if(spawn){
				AiController newAi = aiSystem.SpawnAI(position);
				ais.Add(newAi);
			}
		}
	}

	private void ResetSystem(){
		foreach(AiController ai in ais){
			aiSystem.ClearAI(ai);
		}

		ais.Clear();
		time = 0f;
	}

	private void ToogleSystem(bool activate, bool reset = true){
		useFollow = activate;

		if(reset){
			ResetSystem();
		}
	}

	private void ClearAIEvent(){
		ais.Clear();
		time = 0f;
	}
}
