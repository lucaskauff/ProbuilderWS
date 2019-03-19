using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BoostDisplay : MonoBehaviour {

	private CanvasGroup group;

	private PlayerCharacter player;

	private void Awake() {
		player = FindObjectOfType<PlayerCharacter>();
		group = GetComponent<CanvasGroup>();
	}

	private void Update() {
		if((player.ActualState == PlayerCharacter.State.BOOST) || (player.ActualState == PlayerCharacter.State.AIR_BOOST)){
			group.alpha = 1f;
		}
		else{
			group.alpha = 0f;
		}
	}
}
