using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconInputCalculator {

	private struct CalculatorResult{
		public Vector3 axe;
		public float value;
		public float inverseSign;

		public static readonly CalculatorResult Zero = new CalculatorResult();
	}

	public static float GetHorizontalOneAxe(Transform joyconSimulator, CustomInputManager.JoyconAxe axe){
		CalculatorResult result = GetAxeValues(joyconSimulator, axe);

		if(result.inverseSign < 0){
			result.value = result.value / 2f;
		}
		else{
			float sign = Mathf.Sign(result.value);
			result.value = sign * (1f - Mathf.Abs(result.value) / 2f);
		}

		return result.value;
	}

	public static float GetHorizontalTwoAxes(Transform joyconSimulator, CustomInputManager.JoyconAxe axe1, CustomInputManager.JoyconAxe axe2){
		return 0f;
	}

	public static float GetHorizontalTwoJoycons(Transform leftJoycon, Transform rightJoycon, CustomInputManager.JoyconAxe axe){
		return 0f;
	}

	private static CalculatorResult GetAxeValues(Transform joyconSimulator, CustomInputManager.JoyconAxe axe){
		CalculatorResult result;

		switch(axe){
			case CustomInputManager.JoyconAxe.FORWARD:
				result.axe = joyconSimulator.forward;
				result.value = result.axe.y;
				result.inverseSign = result.axe.x - result.axe.z;
				break;

			case CustomInputManager.JoyconAxe.RIGHT:
				result.axe = joyconSimulator.right;
				result.value = -result.axe.y;
				result.inverseSign = result.axe.x - result.axe.z;
				break;

			case CustomInputManager.JoyconAxe.UP:
				result.axe = joyconSimulator.up;
				result.value = result.axe.y;
				result.inverseSign = result.axe.x - result.axe.z;
				break;

			default:
				result.axe = Vector3.zero;
				result.value = 0f;
				result.inverseSign = 0f;
				break;
		}

		return result;
	}
}
