﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAITrigger : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Player"){
			AiSystem.Instance.ClearAIs();
		}
	}
}
