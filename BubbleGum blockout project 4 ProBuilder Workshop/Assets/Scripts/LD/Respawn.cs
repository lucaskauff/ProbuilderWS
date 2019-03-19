using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

	[SerializeField] private Transform spawn = null;

	private BikeCharacter player;

	private void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
	}

	private void OnTriggerEnter(Collider other) {
		if((other.tag == "Player") || (other.tag == "AI")){
			DoRespawn(other.GetComponent<PlayerCharacter>());
		}
	}

	public void DoRespawn(){
		DoRespawn(player);
	}

	public void DoRespawn(BikeCharacter target){
		PlayerCharacter player = target as PlayerCharacter;
		if(player != null){
			player.BikeComponents.Transform.position = spawn.position;
			player.BikeComponents.Transform.rotation = spawn.rotation;
			player.PlayerComponents.Rigidbody.velocity = Vector3.zero;
			player.PlayerComponents.Painter.Desactivate();
		}
		else{
			target.parentTransform.position = spawn.position;
			target.parentTransform.rotation = spawn.rotation;
			target.BikeComponents.Transform.localRotation = Quaternion.identity;
		}
	}
}
