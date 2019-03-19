using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickAction : MonoBehaviour {

	[SerializeField] private Camera mainCamera = null;
	[SerializeField] private bool onDown = true;
	[SerializeField] private bool debugLine = true;
	[SerializeField] private float lineDuration = 5f;

	private Transform cameraTransform;

	private void Awake() {
		cameraTransform = mainCamera.GetComponent<Transform>();
	}

	private void Update() {
		bool click;
		if(onDown){
			click = Input.GetMouseButtonDown(0);
		}
		else{
			click = Input.GetMouseButton(0);
		}

		if(click){
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray, out hit)){
				if(debugLine){
					Debug.DrawLine(cameraTransform.position, hit.point, Color.red, lineDuration);
				}

				ReceiveAction action = hit.collider.GetComponent<ReceiveAction>();
				action?.MakeAction(hit.point);
			}
		}
	}
}
