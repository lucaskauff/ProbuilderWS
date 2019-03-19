using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class RoomDoor : MonoBehaviour {

	public struct MoveInfo{
		public Vector3 spawn;
		public Transform objective;
	}
	public delegate void RoomDelegate(RoomDoor door, Transform player);
	public event RoomDelegate RoomEnterEvent;
	public event RoomDelegate RoomExitEvent;

	public bool spawn = true;

	public Transform SelfTransform {get; private set;}

	private Transform[] spawnTransforms;
	private Transform[] objectiveTransforms;

	private void Awake() {
		SelfTransform = GetComponent<Transform>();
		SetInformations();
	}

	private void SetInformations(){
		spawnTransforms = new Transform[SelfTransform.childCount];
		objectiveTransforms = new Transform[SelfTransform.childCount];
		for(int i = 0; i < SelfTransform.childCount; i++){
			Transform child = SelfTransform.GetChild(i);
			spawnTransforms[i] = child;

			if(child.childCount > 0){
				objectiveTransforms[i] = child.GetChild(0);
			}
			else{
				objectiveTransforms[i] = SelfTransform;
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Player"){
			RoomEnterEvent?.Invoke(this, other.GetComponent<Transform>());
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.tag == "Player"){
			RoomExitEvent?.Invoke(this, other.GetComponent<Transform>());
		}
	}

	public MoveInfo[] GetSpawns(){
		MoveInfo[] result = new MoveInfo[spawnTransforms.Length];
		for(int i = 0; i < result.Length; i++){
			result[i].spawn = spawnTransforms[i].position;
			result[i].objective = objectiveTransforms[i];
		}

		return result;
	}

	#if UNITY_EDITOR

	[Header("Debug")]
	[SerializeField] private Color individualColor = new Color();
	[SerializeField] private Color spawnColor = new Color();
	[SerializeField] private float lineSize = 0f;

	public void DrawExternGizmos(Color color){
		if(IsSelected()){
			DrawGizmos(individualColor);
		}
		else{
			DrawGizmos(color);
		}

	}

	private void DrawGizmos(Color color){

		SelfTransform = GetComponent<Transform>();
		SetInformations();

		BoxCollider box = GetComponent<BoxCollider>();

		Gizmos.color = color;
		Matrix4x4 rotationMatrix = Matrix4x4.TRS(SelfTransform.position, SelfTransform.rotation, SelfTransform.lossyScale);
		Gizmos.matrix = rotationMatrix;

		if(box != null){
			Gizmos.DrawCube(box.center, box.size);
		}

		Gizmos.color = spawnColor;
		Handles.color = spawnColor;

		for(int i = 0; i < spawnTransforms.Length; i++){
			Gizmos.DrawSphere(spawnTransforms[i].localPosition, 0.6f);
			Handles.DrawAAPolyLine(lineSize, new Vector3[]{objectiveTransforms[i].position, spawnTransforms[i].position});
		}
	}

	private bool IsSelected(){
		bool selected = (Selection.activeObject == gameObject);

		if(spawnTransforms != null){
			for(int i = 0; i < spawnTransforms.Length; i++){
				selected = selected || (Selection.activeObject == spawnTransforms[i].gameObject);
			}
		}

		return selected;
	}


	#endif

}
