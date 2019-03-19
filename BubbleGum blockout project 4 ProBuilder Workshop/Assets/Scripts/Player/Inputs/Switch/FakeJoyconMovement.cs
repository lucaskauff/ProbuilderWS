using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeJoyconMovement : MonoBehaviour {

	private enum FakeState{STATIC, TURNED, REDUCED, READY}

	[SerializeField] private FakeState firstState = FakeState.STATIC;

	[SerializeField] private Transform leftJoyconRotation = null;
	[SerializeField] private Transform rightTransformRotation = null;

	[Space]

	[SerializeField] private Animator joycon3dAnimator = null;
	[SerializeField] private Animator joyconUIAnimator = null;

	private CustomInputManager inputManager;

	private void Awake() {
		inputManager = CustomInputManager.Instance;
		StartCoroutine(MoveJoyconsCoroutine());
	}

	private IEnumerator MoveJoyconsCoroutine(){
		if(firstState == FakeState.STATIC){
			yield return new WaitUntil(() => {return inputManager.GetFakeAnimationDown();});
		}
		joycon3dAnimator.SetTrigger("Turn");

		if(firstState <= FakeState.TURNED){
			yield return null;
			yield return new WaitUntil(() => {return inputManager.GetFakeAnimationDown();});
		}
		
		joyconUIAnimator.SetTrigger("Reduce");

		if(firstState <= FakeState.REDUCED){
			yield return null;
			yield return new WaitUntil(() => {return inputManager.GetFakeAnimationDown();});
		}
		
		joycon3dAnimator.enabled = false;
		Vector3 leftRotation = new Vector3(90f, 0f, 0f);
		Vector3 rightRotation = new Vector3(-90f, 0f, 0f);
		while(true){
			float horizontal = inputManager.GetJoyconRawHorizontal();
			leftRotation.z = -horizontal * 180f;
			rightRotation.z = horizontal * 180f;
			leftJoyconRotation.localRotation = Quaternion.Euler(leftRotation);
			rightTransformRotation.localRotation = Quaternion.Euler(rightRotation);
			yield return null;
		}
	}
}
