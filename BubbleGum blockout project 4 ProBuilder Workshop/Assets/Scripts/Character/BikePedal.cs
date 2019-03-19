using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikePedal : MonoBehaviour {


	[SerializeField] private Transform targetTransform = null;

	private Transform selfTransform;


	private void Awake() {
		selfTransform = GetComponent<Transform>();
	}

	private void Update() {
		selfTransform.position = targetTransform.position;
	}
}
