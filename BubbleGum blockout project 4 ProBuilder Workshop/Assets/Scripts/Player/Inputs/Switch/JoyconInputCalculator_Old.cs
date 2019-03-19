using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconInputCalculator_Old {

	public static Vector2 GetFloatDirections(List<Joycon> joycons){

		if(joycons.Count < 2){
			return Vector2.zero;
		}

		return Vector2.zero;
	}

	public static Vector3 GetMixedDirections(List<Joycon> joycons, Vector3 direction){
		if(joycons.Count < 2){
			return Vector3.zero;
		}

		Vector3 directions = GetJoyconDirection(joycons[0], direction);
		directions += GetJoyconDirection(joycons[1], direction);

		return directions.normalized;
	}

	public static Vector3 GetJoyconDirection(Joycon joycon, Vector3 direction){
		return joycon.GetVector() * direction;
	}

	public static float GetHorizontal(Joycon joycon){
		return GetJoyconDirection(joycon, Vector3.forward).z;
	}

	public static float GetOther(Joycon joycon){
		return GetJoyconDirection(joycon, Vector3.forward).y;
	}
}
