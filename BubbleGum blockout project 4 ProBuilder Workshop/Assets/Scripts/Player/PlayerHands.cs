using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour {

	public static readonly int handLayer = 15;

	public CollisionGetter leftBox;
	public CollisionGetter rightBox;

	private CustomInputManager customInputManager;

	private void Awake() {
		customInputManager = CustomInputManager.Instance;

		Renderer leftRenderer = leftBox.GetComponent<Renderer>();
		Renderer rightRenderer = rightBox.GetComponent<Renderer>();
		Material leftMaterial = leftRenderer.material;
		Material rightMaterial = rightRenderer.material;

		Color leftColor = customInputManager.GetJoyconColor(CustomInputManager.JoyconType.LEFT);
		Color rightColor = customInputManager.GetJoyconColor(CustomInputManager.JoyconType.RIGHT);
		leftMaterial.SetColor("_TintColor", leftColor);
		rightMaterial.SetColor("_TintColor", rightColor);

		leftRenderer.material = leftMaterial;
		rightRenderer.material = rightMaterial;

		leftBox.gameObject.SetActive(false);
		rightBox.gameObject.SetActive(false);
	}

	private void Update() {
		CustomInputManager.InputAsk joyconType = customInputManager.GetHandUp();
		leftBox.gameObject.SetActive((joyconType == CustomInputManager.InputAsk.LEFT) || (joyconType == CustomInputManager.InputAsk.BOTH));
		rightBox.gameObject.SetActive((joyconType == CustomInputManager.InputAsk.RIGHT) || (joyconType == CustomInputManager.InputAsk.BOTH));
	}
}
