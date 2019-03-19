using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class Room : MonoBehaviour {

	public RoomDoor[] doors = null;

	[SerializeField] private LayerMask doorLayer = 0;

	private Vector3 barycentre;

	private bool inRoom;

	private Dictionary<RoomDoor, RoomDoor.MoveInfo[]> roomSpawns;

	private List<AiController> ais;

	private void Awake() {
		barycentre = CalculateBarycentre();

		roomSpawns = new Dictionary<RoomDoor, RoomDoor.MoveInfo[]>();
		ais = new List<AiController>();
	}

	private void Start() {
		for(int i = 0; i < doors.Length; i++){
			if(doors[i] == null){
				continue;
			}

			doors[i].RoomExitEvent += OnDoorExit;
			roomSpawns.Add(doors[i], doors[i].GetSpawns());
		}
	}

	public void EnterRoom(RoomDoor door){
		inRoom = true;

		int holl = Random.Range(0, roomSpawns.Count - 1);
		int i = 0;
		foreach(RoomDoor targetDoor in roomSpawns.Keys){
			if(targetDoor.Equals(door)){
				for(int j = 0; j < roomSpawns[targetDoor].Length; j++){
					AiController ai = AiSystem.Instance.SpawnAI(roomSpawns[targetDoor][j].spawn);
					ai.SetTarget(roomSpawns[targetDoor][j].objective);
					ais.Add(ai);
				}
				continue;
			}

			if(i != holl){
				for(int j = 0; j < roomSpawns[targetDoor].Length; j++){
					AiController ai = AiSystem.Instance.SpawnAI(roomSpawns[targetDoor][j].spawn);
					ai.SetTarget(roomSpawns[targetDoor][j].objective);
					ais.Add(ai);
				}
			}

			i++;
		}
	}
	public void ExitRoom(RoomDoor door){
		inRoom = false;
		for(int i = 0; i < ais.Count; i++){
			AiSystem.Instance.ClearAI(ais[i]);
		}

		ais.Clear();
	}

	private void OnDoorExit(RoomDoor door, Transform player){
		Vector3 playerPosition = player.position;
		Vector3 diff = barycentre - playerPosition;

		// if(Vector3.Distance(barycentre, playerPosition) > Vector3.Distance(barycentre, door.SelfTransform.position)){
		if(Physics.Raycast(playerPosition, diff.normalized, diff.magnitude, doorLayer)){
			if(inRoom){
				ExitRoom(door);
			}
		}
		else if(!inRoom && door.spawn){
			EnterRoom(door);
		}
	}

	private Vector3 CalculateBarycentre(){
		Vector3 barycentre = Vector3.zero;
		
		foreach(RoomDoor rd in doors){
			if(rd == null){
				continue;
			}
			barycentre += rd.GetComponent<Transform>().position;
		}

		return barycentre / (float) doors.Length;
	}

	#if UNITY_EDITOR

	[Header("Debug")]
	[SerializeField] private Color debugColor = Color.green;
	[SerializeField] private Color nameColor = Color.green;
	[SerializeField] private float lineSize = 2f;

	public void DrawGizmos(bool displayRoomText) {
		if(doors.Length < 2){
			return;
		}

		Vector3 barycentre = CalculateBarycentre();
		Gizmos.color = debugColor;
		Handles.color = debugColor;

		GUIStyle textStyle = new GUIStyle();
		textStyle.alignment = TextAnchor.MiddleCenter;
		textStyle.normal.textColor = nameColor;

		foreach(RoomDoor rd in doors){
			if(rd == null){
				continue;
			}
			Handles.DrawAAPolyLine(lineSize, new Vector3[]{barycentre, rd.GetComponent<Transform>().position});
			
			if(displayRoomText){
				Handles.Label(barycentre + Vector3.up * 0.1f, name, textStyle);
			}
		}
	}

	#endif
}
