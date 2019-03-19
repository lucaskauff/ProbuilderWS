using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Old : MonoBehaviour {

	[SerializeField] private int nbHold = 0;

	[SerializeField] private Transform[] enemySpawns = null;
	[SerializeField] private Transform[] outs = null;

	private bool alreadyTriggered;

	private void Awake() {
		alreadyTriggered = false;
	}

	private void Update() {
		if(CustomInputManager.Instance.GetClearAIsDown()){
			alreadyTriggered = false;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if((other.tag != "Player") || alreadyTriggered){
			return;
		}

		int[] holds = new int[nbHold];

		for(int i = 0; i < nbHold; i++){
			int number;
			do{
				number = Random.Range(0, outs.Length);
			}while(AiSpawnTrigger.NumberAlreadyUsed(number, holds, i));

			holds[i] = number;
		}

		for(int i = 0; i < enemySpawns.Length; i++){
			bool isHold = false;

			for(int j = 0; j < nbHold; j++){
				if(holds[j] == i){
					isHold = true;
					break;
				}
			}

			if(isHold){
				continue;
			}

			AiSystem.Instance.SpawnAI(enemySpawns[i].position, outs[i], false);
		}

		alreadyTriggered = true;
	}
}
