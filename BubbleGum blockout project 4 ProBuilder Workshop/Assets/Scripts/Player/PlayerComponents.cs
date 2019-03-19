using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintingSystem;

[System.Serializable]
public struct PlayerComponents {

	public GroundPainter Painter;
	public Rigidbody Rigidbody {get; private set;}
	public PlayerStrike Strike {get; private set;}
	public PlayerHands Hands {get; private set;}
	public PlayerTricks Tricks {get; private set;}
	public PlayerFeedback Feedbacks {get; private set;}

	public void SetComponents(GameObject parent){
		Rigidbody = parent.GetComponent<Rigidbody>();
		Strike = parent.GetComponent<PlayerStrike>();
		Hands = parent.GetComponent<PlayerHands>();
		Tricks = parent.GetComponent<PlayerTricks>();
		Feedbacks = parent.GetComponent<PlayerFeedback>();
	}
}
