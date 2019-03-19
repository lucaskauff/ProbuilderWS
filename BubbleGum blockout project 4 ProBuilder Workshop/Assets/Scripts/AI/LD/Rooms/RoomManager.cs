using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class RoomManager : MonoBehaviour {

	[SerializeField] private Transform roomParent = null;

	private List<Room> rooms;

	private void Awake() {
		SetRooms();
	}

	private void SetRooms(){
		if(roomParent == null){
			return;
		}

		rooms = new List<Room>();

		for(int i = 0; i < roomParent.childCount; i++){
			Room r = roomParent.GetChild(i).GetComponent<Room>();
			if(r != null){
				rooms.Add(r);
			}
		}
	}

	#if UNITY_EDITOR

	[Header("Debug")]
	[SerializeField] private bool displayDoors = false;
	[SerializeField] private bool displayDoorNames = false;
	[SerializeField] private bool displayRooms = false;
	[SerializeField] private bool displayRoomNames = false;
	[SerializeField] private Transform doorParent = null;
	[SerializeField] private Color boxColor = new Color();
	[SerializeField] private float textHeight = 0f;
	[SerializeField] private Color doorNameColor = new Color();

	

	private void OnDrawGizmos() {
		SetRooms();

		if((doorParent == null) || (roomParent == null)){
			return;
		}

		if(!displayDoorNames && !displayDoors && !displayRooms && !displayRoomNames){
			return;
		}

		bool selected = (Selection.activeObject == gameObject);
		selected = selected || (Selection.activeObject == doorParent.gameObject);
		selected = selected || (Selection.activeObject == roomParent.gameObject);
		List<RoomDoor> doors = new List<RoomDoor>();

		for(int i = 0; i < roomParent.childCount; i++){
			selected = selected || (Selection.activeObject == roomParent.GetChild(i).gameObject);
		}

		for(int i = 0; i < doorParent.childCount; i++){
			Transform child = doorParent.GetChild(i);
			RoomDoor rd = child.GetComponent<RoomDoor>();
			if(rd == null){
				continue;
			}

			doors.Add(rd);
			selected = selected || (Selection.activeObject == child.gameObject);
			for(int j = 0; j < child.childCount; j++){
				Transform c = child.GetChild(j);
				selected = selected || (Selection.activeObject == c.gameObject);
				if(c.childCount > 0){
					selected = selected || (Selection.activeObject == c.GetChild(0).gameObject);
				}
			}
		}

		if(!selected){
			return;
		}

		if(displayDoors || displayDoorNames){
			Gizmos.color = boxColor;
			GUIStyle textStyle = new GUIStyle();
			textStyle.alignment = TextAnchor.MiddleCenter;
			textStyle.normal.textColor = doorNameColor;
			Vector3 directionVector = Vector3.up;

			Camera camera = Camera.current;
			if(camera != null){
				directionVector = camera.GetComponent<Transform>().up;
			}
			

			for(int i = 0; i < doors.Count; i++){
				string name = doors[i].gameObject.name;
				BoxCollider box = doors[i].GetComponent<BoxCollider>();
				if(box != null){
					if(displayDoors){
						doors[i].DrawExternGizmos(boxColor);
					}
					
					if(displayDoorNames){
						Handles.Label(doors[i].transform.position + directionVector * box.size.y + directionVector * textHeight, name, textStyle);
					}
				}
				
			}
		}

		if(displayRooms){
			foreach(Room r in rooms){
				r.DrawGizmos(displayRoomNames);
			}
		}
	}

	#endif
}
