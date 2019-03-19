using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
	public class AudioPulling : MonoBehaviour {

		[SerializeField] private AudioObject objectPrefab = null;
		[SerializeField] private int beginPullLength = 0;

		[SerializeField] private Transform audioPullingParent = null;

		private Stack<AudioObject> freeObjects;

		private void Awake() {
			freeObjects = new Stack<AudioObject>();

			if(objectPrefab == null){
				Debug.LogWarning("[AudioSystem - AudioPulling] No Audio Object prefab");
			}

			for(int i = 0; i < beginPullLength; i++){
				AudioObject newObject = Instantiate<AudioObject>(objectPrefab, audioPullingParent);
				freeObjects.Push(newObject);
			}
		}

		public AudioObject AskAudioObject(Transform parent){
			if(freeObjects.Count != 0){
				AudioObject newObject = freeObjects.Pop();
				newObject.SelfTransform.parent = parent;
				newObject.SelfTransform.localPosition = Vector3.zero;
				return newObject;
			}
			else{
				AudioObject newObject = Instantiate<AudioObject>(objectPrefab, parent);
				return newObject;
			}
		}

		public void ReleaseAudioObject(AudioObject obj){
			freeObjects.Push(obj);
			obj.SelfTransform.parent = audioPullingParent;
			obj.SelfTransform.localPosition = Vector3.zero;
		}
	}
}

