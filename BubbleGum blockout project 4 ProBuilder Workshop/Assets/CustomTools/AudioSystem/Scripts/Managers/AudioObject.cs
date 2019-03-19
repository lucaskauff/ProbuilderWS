using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem{

	[System.Serializable]
	public struct ClipInformations{
		public AudioClip clip;
		[Range(0f, 1f)] public float volume;
		[Range(0f, 1f)] public float pitch;

		public void Reset(){
			clip = null;
			volume = 1f;
			pitch = 1f;
		}
	}

	[System.Serializable]
	public struct AudioSourceInformations{
		public AudioManager.MixerType mixerType;
		public bool playOnAwake;
		public bool loop;

		[Range(0, 256)] public int priority;
		[Range(-1f, 1f)] public float stereoPan;
		[Range(0f, 1f)] public float spatialBlend;
		[Range(0f, 1.1f)] public float reverb;

		public ClipInformations[] clips;

		public AudioSourceInformations(int priority){
			clips = new ClipInformations[1];
			clips[0].Reset();

			mixerType = AudioManager.MixerType.SOUND;
			playOnAwake = false;
			loop = false;

			this.priority = priority;
			stereoPan = 0f;
			spatialBlend = 0f;
			reverb = 0f;
		}

		public void Reset() {
			clips = new ClipInformations[1];
			clips[0].Reset();

			mixerType = AudioManager.MixerType.SOUND;
			playOnAwake = false;
			loop = false;

			priority = 128;
			stereoPan = 0f;
			spatialBlend = 0f;
			reverb = 0f;
		}
	}

	[RequireComponent(typeof(AudioSource))]
	public class AudioObject : MonoBehaviour {

		public Transform SelfTransform {get; private set;}

		private AudioSource source;
		private Sound owner;

		private bool hasBegan;

		private void Awake() {
			source = GetComponent<AudioSource>();
			SelfTransform = GetComponent<Transform>();
			hasBegan = false;
		}

		private void Update() {
			if(hasBegan && !source.isPlaying){
				this.owner.StopSound(this);
			}
		}

		public void Play(Sound owner){
			hasBegan = true;
			this.owner = owner;
			source.Play();
		}

		public void Stop(){
			this.owner = null;
			hasBegan = false;
			source.Stop();
		}

		public void SetInformations(AudioSourceInformations informations, int clipId){
			source.clip = informations.clips[clipId].clip;
			source.outputAudioMixerGroup = AudioManager.Instance.GetAudioMixer(informations.mixerType);
			source.loop = informations.loop;
			source.volume = informations.clips[clipId].volume;
			source.pitch = informations.clips[clipId].pitch;
			source.priority = informations.priority;
			source.panStereo = informations.stereoPan;
			source.spatialBlend = informations.spatialBlend;
			source.reverbZoneMix = informations.reverb;
		}
	}
}

