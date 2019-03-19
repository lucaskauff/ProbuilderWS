using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AudioSystem{

	[RequireComponent(typeof(AudioSource))]
	public class SurfaceSong : MonoBehaviour {

		[SerializeField] private SurfaceManager surfaceManager = null;

		[Header("Surface Check")]
		[SerializeField] private Transform checkPosition = null;
		[SerializeField] private float raycastLength = 0f;
		[SerializeField] private LayerMask ignoredLayers = 0;

		private AudioSource source;

		private bool isPlaying;

		private int actualId;

		private void Awake() {
			actualId = 0;
			source = GetComponent<AudioSource>();
			source.loop = true;
		}

		private void Update() {
			if(!isPlaying){
				return;
			}

			ApplySurfaceSong();
		}

		public void Play(){
			if(isPlaying){
				return;
			}

			isPlaying = true;
			ApplySurfaceSong(true);
		}

		public void Stop(){
			if(!isPlaying){
				return;
			}

			isPlaying = false;
			source.Stop();
		}

		public int GetActualSurfaceId(){
			if(checkPosition == null){
				return -2;
			}

			RaycastHit hit;
			if(Physics.Raycast(checkPosition.position, -checkPosition.up, out hit, raycastLength, ~ignoredLayers)){
				for(int i = 0; i < surfaceManager.materials.Length; i++){
					if(hit.collider.material == null){
						continue;
					}

					if(hit.collider.material.name.Equals(surfaceManager.materials[i].name + " (Instance)")){
						return i;
					}
				}
			}
			
			return -1;
		}

		private void ApplySurfaceSong(bool force = false){
			ApplySurfaceSong(GetActualSurfaceId());
		}

		private void ApplySurfaceSong(int id, bool force = false){
			if(id < -1){
				return;
			}

			if(!force && (actualId == id)){
				return;
			}

			ClipInformations clip;

			if(id == -1){
				clip = surfaceManager.defaultClip;
			}
			else{
				if(surfaceManager.clips.Length <= id){
					clip = surfaceManager.defaultClip;
				}
				else{
					clip = surfaceManager.clips[id];
				}
			}

			actualId = id;

			source.Stop();
			source.clip = clip.clip;
			source.volume = clip.volume;
			source.pitch = clip.pitch;
			source.Play();
		}

		#if UNITY_EDITOR

		private void OnDrawGizmos() {
			if(checkPosition == null){
				return;
			}

			if((Selection.activeObject != gameObject) && (Selection.activeObject != checkPosition.gameObject)){
				return;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawLine(checkPosition.position, checkPosition.position + -checkPosition.up * raycastLength);
		}

		#endif
	}
}

