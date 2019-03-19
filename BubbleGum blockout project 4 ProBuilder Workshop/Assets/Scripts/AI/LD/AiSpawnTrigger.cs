using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSpawnTrigger : MonoBehaviour {

	[SerializeField] private bool triggerOnce = false;
	[SerializeField] private bool useCircle = false;

	[Header("Circle Spawn")]
	[SerializeField] private int nbCircleEnemy = 0;
	[SerializeField] private int nbCircleHold = 0;
	[SerializeField] private bool holdNext = false;
	[SerializeField] private bool holdCenter = false;
	[SerializeField] private float circleRadius = 0f;
	[SerializeField] private float spawnAngle = 0f;

	[SerializeField] private Transform[] spawns = null;

	private bool alreadyTrigger;

	private AiSystem aISystem;

	private void Awake() {
		alreadyTrigger = false;
		aISystem = AiSystem.Instance;
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag != "Player"){
			return;
		}

		if(triggerOnce && alreadyTrigger){
			return;
		}

		if(useCircle){
			SpawnAICircle();
		}
		else{
			foreach(Transform t in spawns){
				aISystem.SpawnAI(t.position);
			}
		}

		alreadyTrigger = true;
	}

	public void SpawnAICircle(){
		SpawnEnemyCircle(nbCircleEnemy, nbCircleHold, holdNext, circleRadius, spawnAngle);
	}

	public void SpawnEnemyCircle(int nbCircleEnemy, int nbCircleHold, bool holdNext, float circleRadius, float spawnAngle){
		int[] holdPlaces = new int[nbCircleHold];

		if(holdCenter){
			int begin = (nbCircleEnemy - nbCircleHold) / 2;
			for(int i = 0; i < nbCircleHold; i++){
				holdPlaces[i] = begin + i;
			}
		}
		else if(holdNext){
			int begin = Random.Range(0, nbCircleEnemy - nbCircleHold);
			for(int i = 0; i < nbCircleHold; i++){
				holdPlaces[i] = begin + i;
			}
		}
		else{
			for(int i = 0; i < nbCircleHold; i++){
				int number;
				do{
					number = Random.Range(0, nbCircleEnemy);
				}while(NumberAlreadyUsed(number, holdPlaces, i));

				holdPlaces[i] = number;
			}
		}

		for(int i = 0; i < nbCircleEnemy; i++){
			bool isHold = false;

			for(int j = 0; j < nbCircleHold; j++){
				if(holdPlaces[j] == i){
					isHold = true;
					break;
				}
			}

			if(isHold){
				continue;
			}

			float convertSpawnAngle = spawnAngle * Mathf.Deg2Rad;
			float actualRotation = aISystem.Player.BikeComponents.Transform.rotation.eulerAngles.z * Mathf.Deg2Rad + (Mathf.PI / 2f);

			Vector3 position = aISystem.Player.bikeCenter.position;
			float angle = ((i * convertSpawnAngle) / nbCircleEnemy);
			angle = angle + actualRotation - (convertSpawnAngle / 2f);
			position.x += Mathf.Cos(angle) * circleRadius;
			position.z += Mathf.Sin(angle) * circleRadius;

			aISystem.SpawnAI(position);
		}

	}

	public static bool NumberAlreadyUsed(int number, int[] array, int index){
		for(int i = 0; i < index; i++){
			if(array[i] == number){
				return true;
			}
		}

		return false;
	}

	public void ResetTrigger(){
		alreadyTrigger = false;
	}
}
