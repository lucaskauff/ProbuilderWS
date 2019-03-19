using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPulling : MonoBehaviour {

	[SerializeField] private AiController aiPrefab = null;
	[SerializeField] private int beginPullLength = 0;

	[SerializeField] private Transform aiPullingParent = null;

	private Stack<AiController> freeObjects;

	private void Awake() {
		freeObjects = new Stack<AiController>();

		if(aiPrefab == null){
			Debug.LogWarning("[AiSystem - AiPulling] No AI prefab");
		}

		for(int i = 0; i < beginPullLength; i++){
			AiController newObject = Instantiate<AiController>(aiPrefab, aiPullingParent);
			newObject.gameObject.SetActive(false);
			freeObjects.Push(newObject);
		}
	}

	public AiController AskNewAI(Transform parent, Vector3 position, Transform target, bool wait = false){
		AiController newObject;
		if(freeObjects.Count != 0){
			newObject = freeObjects.Pop();
			newObject.gameObject.SetActive(true);
			newObject.SelfTransform.position = position;
			newObject.SelfTransform.rotation = Quaternion.identity;
			newObject.SelfTransform.parent = parent;
		}
		else{
			newObject = Instantiate<AiController>(aiPrefab, position, Quaternion.identity, parent);
			newObject.gameObject.SetActive(true);
		}
		
		newObject.SetTarget(target, wait);
		newObject.NavMeshAgent.enabled = true;

		return newObject;
	}

	public void ReleaseAI(AiController ai){
		freeObjects.Push(ai);
		ai.NavMeshAgent.enabled = false;
		ai.SelfTransform.parent = aiPullingParent;
		ai.SelfTransform.localPosition = Vector3.zero;
		ai.gameObject.SetActive(false);
	}
}
