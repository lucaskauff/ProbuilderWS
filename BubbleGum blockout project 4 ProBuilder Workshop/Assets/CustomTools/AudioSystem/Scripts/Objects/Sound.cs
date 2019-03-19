using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AudioSystem
{
	[CreateAssetMenu(fileName = "Sound", menuName = "BubbleGum/Sound", order = 2)]
	public class Sound : ScriptableObject {

		private Transform target = null;
		
		[SerializeField] private bool randomSong = false;
		[SerializeField] private bool overrideSelf = false;

		[HideIf("overrideSelf"), HideIf("randomSong")]
		[SerializeField] private bool useLikePlaylist = false;
		

		[HideIf("randomSong")]
		[SerializeField] private bool loop = false;

		[SerializeField] private AudioSourceInformations informations = new AudioSourceInformations(128);

		private List<AudioObject> playingSounds;

		private int actualId;

		public virtual void Init(Transform target) {
			playingSounds = new List<AudioObject>();
			this.target = target;

			if(informations.playOnAwake){
				PlaySound();
			}
		}

		public virtual void PlaySound(){
			if((informations.clips == null) || (informations.clips.Length == 0)){
				return;
			}

			if(overrideSelf){
				StopSound();
			}

			int id;

			if(randomSong){
				id = Random.Range(0, informations.clips.Length);
			}
			else{
				if(loop){
					id = actualId % informations.clips.Length;
				}
				else{
					if(actualId >= informations.clips.Length){
						id = informations.clips.Length - 1;
					}
					else{
						id = actualId;
					}
				}
				actualId++;
			}

			AudioObject newSound = AudioManager.Instance.RegisterSong(this);
			newSound.SetInformations(informations, id);

			playingSounds.Add(newSound);
			newSound.Play(this);
		}

		public virtual void StopSound(){
			playingSounds.Clear();
			AudioManager.Instance.ReleaseSong(this);
		}

		public virtual bool IsPlaying(){
			return (playingSounds.Count != 0);
		}

		public Transform GetTarget(){
			return target;
		}

		public virtual void StopSound(AudioObject obj){
			if(!playingSounds.Contains(obj)){
				return;
			}

			playingSounds.Remove(obj);
			AudioManager.Instance.ReleaseSong(this, obj);

			if(!overrideSelf && useLikePlaylist){
				if(actualId < informations.clips.Length){
					PlaySound();
				}
			}
		}

		public virtual void ResetSound(){
			actualId = 0;
		}

		protected virtual void Reset() {
			target = null;
			overrideSelf = true;
			randomSong = true;
			informations.Reset();
		}
	}

}

