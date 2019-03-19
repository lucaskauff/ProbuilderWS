using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace AudioSystem
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicSwitcher : SerializedMonoBehaviour {

		[System.Serializable]
		private struct MusicInformations{
			public AudioClip clip;

			[Range(0f, 1f)] 
			public float volumeMax;

			[Range(0f, 1f)] 
			public float pitch;

			public MusicInformations(AudioClip clip, float volumeMax, float pitch){
				this.clip = clip;
				this.volumeMax = volumeMax;
				this.pitch = pitch;
			}
		}

		public enum MusicType {NONE, MAIN_MENU, IN_GAME}

		[OdinSerialize] private Dictionary<MusicType, MusicInformations> musics;

		[Space]

		[SerializeField] private float transitionTime = 0f;

		private AudioSource source;

		private MusicType actualMusic;
		private Coroutine musicCoroutine;

		private float timer;

		private void Awake() {
			musics = new Dictionary<MusicType, MusicInformations>();

			source = GetComponent<AudioSource>();

			timer = 0f;
			SwitchMusic(MusicType.MAIN_MENU, true);
		}

		public void SwitchMusic(MusicType type, bool cut = false){
			if(musicCoroutine != null){
				StopCoroutine(musicCoroutine);
			}

			musicCoroutine = StartCoroutine(SwitchMusicCoroutine(type, cut));
		}

		private IEnumerator SwitchMusicCoroutine(MusicType type, bool cut){
			float lastMusicVolume;

			if(musics.ContainsKey(actualMusic)){
				lastMusicVolume = musics[actualMusic].volumeMax;
			}
			else{
				lastMusicVolume = 0f;
			}

			while(timer > 0f){
				yield return null;
				timer -= Time.deltaTime;
				source.volume = (timer / (transitionTime * 0.5f)) * lastMusicVolume;
			}

			timer = 0f;
			source.volume = 0f;
			source.Stop();

			if(musics.ContainsKey(type)){
				source.clip = musics[type].clip;
				source.pitch = musics[type].pitch;
				actualMusic = type;
				source.Play();

				if(!cut){
					while(timer < (transitionTime * 0.5f)){
						yield return null;
						timer += Time.deltaTime;
						source.volume = (timer / (transitionTime * 0.5f)) * musics[type].volumeMax;
					}
				}

				timer = transitionTime * 0.5f;
				source.volume = musics[type].volumeMax;
				
			}
			else{
				actualMusic = MusicType.NONE;
			}

			
		}
	}
}

