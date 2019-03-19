using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugCamera : MonoBehaviour {

	[SerializeField] private bool useDebug;

	private Transform selfTransform;
	private Transform sceneCameraTransform;

	#if UNITY_EDITOR

	private void Awake() {
		// selfTransform = GetComponent<Transform>();
		SceneView.lastActiveSceneView.FrameSelected();
	}

	void Update () {
		// if(sceneCameraTransform == null){
		// 	Camera sceneCamera = ShortcutEditor.GetSceneCamera();
		// 	if(sceneCamera == null){
		// 		return;
		// 	}

		// 	sceneCameraTransform = sceneCamera.GetComponent<Transform>();
		// }

		// sceneCameraTransform.position = selfTransform.position;
		// sceneCameraTransform.rotation = selfTransform.rotation;
	}

	#endif
}
