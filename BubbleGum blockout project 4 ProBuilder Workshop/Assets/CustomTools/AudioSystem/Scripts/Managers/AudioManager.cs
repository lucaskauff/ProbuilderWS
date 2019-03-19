using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem{
	
	[RequireComponent(typeof(MusicSwitcher), typeof(AudioPulling))]
	public class AudioManager : MonoBehaviour {

		public enum MixerType{MUTE, SOUND, MUSIC}

		public static AudioManager Instance {get; private set;}

		[SerializeField] private bool dontDestroyOnLoad = true;

		[Header("Mixers")]
		[SerializeField] private AudioMixerGroup musicMixer = null;
		[SerializeField] private AudioMixerGroup soundMixer = null;
		[SerializeField] private AudioMixerGroup muteMixer = null;

		private Dictionary<Sound, List<AudioObject>> actualSongs;
		private Dictionary<MixerType, AudioMixerGroup> mixers;

		private AudioPulling pulling;
		private MusicSwitcher musicSwitcher;

		private void Awake() {
			if(Instance != null){
				if(Instance != this){
					Destroy(gameObject);
				}
				
				return;
			}

			Instance = this;

			if(dontDestroyOnLoad){
				DontDestroyOnLoad(gameObject);
			}

			pulling = GetComponent<AudioPulling>();
			musicSwitcher = GetComponent<MusicSwitcher>();

			actualSongs = new Dictionary<Sound, List<AudioObject>>();
			mixers = new Dictionary<MixerType, AudioMixerGroup>();

			mixers.Add(MixerType.MUTE, muteMixer);
			mixers.Add(MixerType.MUSIC, musicMixer);
			mixers.Add(MixerType.SOUND, soundMixer);
		}

		public AudioObject RegisterSong(Sound song){
			if(!actualSongs.ContainsKey(song)){
				actualSongs.Add(song, new List<AudioObject>());
			}

			Transform parent = song.GetTarget();

			AudioObject newObject = pulling.AskAudioObject(parent);
			actualSongs[song].Add(newObject);
			return newObject;
		}

		public void ReleaseSong(Sound song){
			if(!actualSongs.ContainsKey(song)){
				return;
			}

			foreach(AudioObject obj in actualSongs[song]){
				obj.Stop();
				pulling.ReleaseAudioObject(obj);
			}

			actualSongs.Remove(song);
		}

		public void ReleaseSong(Sound song, AudioObject obj){
			if(!actualSongs.ContainsKey(song)){
				return;
			}

			if(!actualSongs[song].Contains(obj)){
				return;
			}

			obj.Stop();
			pulling.ReleaseAudioObject(obj);
			actualSongs[song].Remove(obj);

			if(actualSongs[song].Count == 0){
				actualSongs.Remove(song);
			}
		}

		public void SwitchMusic(MusicSwitcher.MusicType musicType, bool cut = false){
			musicSwitcher.SwitchMusic(musicType, cut);
		}

		public AudioMixerGroup GetAudioMixer(MixerType type){
			if(mixers.ContainsKey(type)){
				return mixers[type];
			}

			return null;
		}
	}

}

