using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class JoyconTest : MonoBehaviour {
	// private Joycon leftJoyCon;
	// private Joycon rightJoyCon;

	// private Transform selfTransform;

	public float lowFreq;
	public float highFreq;
	public float duration;
	public AnimationCurve curve;

	public CustomRumble.Rumble smallVibration; 
	public CustomRumble.Rumble bigVibration; 
	public CustomRumble.Rumble continuousVibration;
	public CustomRumble.Rumble continuousVibration2;

	// private Coroutine[] coroutines;

	// private bool continuousRumbles;

	// float time;

	// private Animator animator;

	// private void Awake() {
		// selfTransform = GetComponent<Transform>();
		// List<Joycon> joycons = CustomInputManager.Instance.JoyconManager.joyConList;
		// if(joycons[1].isLeft){
		// 	leftJoyCon = joycons[0];
		// 	rightJoyCon = joycons[1];
		// }
		// else{
		// 	leftJoyCon = joycons[1];
		// 	rightJoyCon = joycons[0];
		// }

		// coroutines = new Coroutine[4];
		// time = 0f;

		// animator = GetComponent<Animator>();
	// }

	private void Update() {
		// if(Mathf.Abs(rightJoyCon.GetAccel().x) > 2f){
		// 	animator.SetTrigger("Up");
		// }

		// if(Mathf.Abs(rightJoyCon.GetAccel().y) > 2f){
		// 	animator.SetTrigger("Left");
		// }

		// if(Mathf.Abs(rightJoyCon.GetAccel().z) > 2f){
		// 	animator.SetTrigger("Right");
		// }

		// time += Time.deltaTime;

		// if(time > duration){
		// 	time -= duration;
		// }

		// if(Input.GetKeyDown(KeyCode.K)){
		// 	continuousRumbles = !continuousRumbles;
		// 	rightJoyCon.SetRumble(0f, 0f, 0f);
		// }

		// if(continuousRumbles){
		// 	rightJoyCon.SetRumble(lowFreq, highFreq, curve.Evaluate(time / duration));
		// }
		// else{
		// 	TestVibration();
		// }

		float test1 = transform.right.x;
		float test2 = transform.right.z;
		float test3 = Mathf.Sign(test1 + test2);
		float test4 = Mathf.Sign(test1 - test2);

		Debug.Log(transform.right + " " + Mathf.Sign(test1) + " " + Mathf.Sign(test2) + " " + test3 + " " + test4);
	}

	// private void TestVibration(){
	// 	if(Input.GetKeyDown(KeyCode.I)){
	// 		if(coroutines[0] == null){
	// 			coroutines[0] = StartCoroutine(RumbleCoroutine(smallVibration, 0));
	// 		}
	// 		else{
	// 			StopCoroutine(coroutines[0]);
	// 			coroutines[0] = null;
	// 			rightJoyCon.SetRumble(0f, 0f, 0f);
	// 		}
	// 	}

	// 	if(Input.GetKeyDown(KeyCode.O)){
	// 		 if(coroutines[1] == null){
	// 			coroutines[1] = StartCoroutine(RumbleCoroutine(bigVibration, 1));
	// 		}
	// 		else{
	// 			StopCoroutine(coroutines[1]);
	// 			coroutines[1] = null;
	// 			rightJoyCon.SetRumble(0f, 0f, 0f);
	// 		}
	// 	}

	// 	if(Input.GetKeyDown(KeyCode.P)){
	// 		 if(coroutines[2] == null){
	// 			coroutines[2] = StartCoroutine(RumbleCoroutine(continuousVibration, 2));
	// 		}
	// 		else{
	// 			StopCoroutine(coroutines[2]);
	// 			coroutines[2] = null;
	// 			rightJoyCon.SetRumble(0f, 0f, 0f);
	// 		}
	// 	}

	// 	if(Input.GetKeyDown(KeyCode.M)){
	// 		 if(coroutines[3] == null){
	// 			coroutines[3] = StartCoroutine(RumbleCoroutine(continuousVibration2, 3));
	// 		}
	// 		else{
	// 			StopCoroutine(coroutines[3]);
	// 			coroutines[3] = null;
	// 			rightJoyCon.SetRumble(0f, 0f, 0f);
	// 		}
	// 	}
	// }

	// private IEnumerator RumbleCoroutine(CustomRumble.Rumble rumble, int coroutineId){
	// 	do{
	// 		float actualTime = 0f;
	// 		while(actualTime < rumble.duration){
	// 			float curveTime = actualTime / rumble.duration;
	// 			rightJoyCon.SetRumble(rumble.lowFreqCurve.Evaluate(curveTime), rumble.highFreqCurve.Evaluate(curveTime), rumble.amplitudeCurv.Evaluate(curveTime));
	// 			yield return null;
	// 			actualTime += Time.deltaTime;
	// 		}
	// 	}while(rumble.loop);
		
	// 	rightJoyCon.SetRumble(0f, 0f, 0f);
	// 	coroutines[coroutineId] = null;
	// 	rightJoyCon.SetRumble(0f, 0f, 0f);
	// }
}
