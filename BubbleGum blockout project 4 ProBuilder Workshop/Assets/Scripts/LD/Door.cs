using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	[SerializeField] private bool manualOpening = false;

	[Header("References")]
	[SerializeField] private Animator doorAnimator = null;

	private List<OpeningDoor> internEntities;

	private void Awake() {
		internEntities = new List<OpeningDoor>();
	}

	private void OnTriggerEnter(Collider other) {
		if(manualOpening){
			return;
		}

		OpeningDoor entity = other.GetComponent<OpeningDoor>();
		if(entity == null){
			return;
		}

		if(internEntities.Contains(entity)){
			return;
		}

		internEntities.Add(entity);
		if(internEntities.Count == 1){
			Open();
		}
	}

	private void OnTriggerExit(Collider other) {
		if(manualOpening){
			return;
		}

		OpeningDoor entity = other.GetComponent<OpeningDoor>();
		if(entity == null){
			return;
		}

		if(!internEntities.Contains(entity)){
			return;
		}

		internEntities.Remove(entity);

		if(internEntities.Count == 0){
			Close();
		}
	}

	public void Open(){
		doorAnimator.SetTrigger("Open");
	}

	public void Close(){
		doorAnimator.SetTrigger("Close");
	}
}
