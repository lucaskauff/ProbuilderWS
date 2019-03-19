using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class CameraManager : SerializedMonoBehaviour {

	public enum CameraType {STOP, FRONT, LEFT, RIGHT, BOOST, AIR, LEFT_DRIFT, RIGHT_DRIFT, BACK}

	[Header("Priority")]
	[SerializeField] private int cameraLowPriority = 5;
	[SerializeField] private int cameraHighPriority = 15;

	[OdinSerialize]
	public Dictionary<CameraType, CinemachineVirtualCamera> cameras;

	private CameraType actualCamera;

	private void Awake() {
		foreach(var cam in cameras){
			if(cam.Key == CameraType.STOP){
				cam.Value.Priority = cameraHighPriority;
			}
			else{
				cam.Value.Priority = cameraLowPriority;
			}
		}
	}

	public void SwitchCamera(CameraType cameraType){
		if(actualCamera == cameraType){
			return;
		}
		
		if(!cameras.ContainsKey(cameraType) || (cameras[cameraType] == null)){
			return;
		}

		if(cameras.ContainsKey(actualCamera)){
			cameras[actualCamera].Priority = cameraLowPriority;
		}

		cameras[cameraType].Priority = cameraHighPriority;
		actualCamera = cameraType;
	}
}
