using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ShortcutEditor {

	[MenuItem("Bubble Gum/Character To Spawn")]
	public static void CharacterToSpawn(){
		GameObject spawn = GetSpawn();
		GameObject character = GetCharacter();
		
		if(spawn == null){
			Debug.LogWarning("Spawn in null");
			return;
		}

		if(character == null){
			Debug.LogWarning("Character in null");
			return;
		}

		TeleportCharacter(character.GetComponent<Transform>(), spawn.GetComponent<Transform>());

	}

	[MenuItem("Bubble Gum/Character To Scene Camera %h")]
	public static void CharacterToView(){
		Camera sceneCamera = GetSceneCamera();
		GameObject character = GetCharacter();
		
		if(sceneCamera == null){
			Debug.LogWarning("Scene Camera is null");
			return;
		}

		if(character == null){
			Debug.LogWarning("Character is null");
			return;
		}

		TeleportCharacter(character.GetComponent<Transform>(), sceneCamera.GetComponent<Transform>());
	}

	// [MenuItem("Bubble Gum/Debug/Joycon Manager Correction &j")]
	public static void JoyconManagerCorrection(){
		string name = "JoyconManager";
		GameObject joyconManager = GameObject.Find(name);
		if(joyconManager != null){
			GameObject.DestroyImmediate(joyconManager);
		}

		joyconManager = new GameObject(name);

		joyconManager.AddComponent(typeof(JoyconManager));
		EditorUtility.SetDirty(joyconManager);
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}

	public static GameObject GetSpawn(){
		return GameObject.FindGameObjectWithTag("Spawn");
	}

	public static GameObject GetCharacter(){
		return GameObject.FindGameObjectWithTag("3C");
	}

	public static Camera GetSceneCamera(){
		if(SceneView.lastActiveSceneView == null){
			return null;
		}
		
		return SceneView.lastActiveSceneView.camera;
	}

	private static void TeleportCharacter(Transform character, Transform target){
		Vector3 characterRotation = character.rotation.eulerAngles;

		character.position = target.position;
		characterRotation.y = target.rotation.eulerAngles.y;
		character.rotation = Quaternion.Euler(characterRotation);
	}
}

#endif