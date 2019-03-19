using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HighFive{
	void MakeFive();
}

public class HighFiveElement : MonoBehaviour, HighFive {

	public void MakeFive(){

	}

	private void Reset() {
		gameObject.layer = PlayerHands.handLayer;
	}
}
