using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using AudioSystem;

public class CharacterSounds : SerializedMonoBehaviour
{
    [System.Serializable]
    private struct SoundState{
        public Sound beginSound;
        public Sound continuousSound;
        public Sound endSound;

        [System.NonSerialized]
        public bool firstState;

        public SoundState(Sound beginSound, Sound continuousSound, Sound endSound){
            this.beginSound = beginSound;
            this.continuousSound = continuousSound;
            this.endSound = endSound;

            firstState = false;
        }
    }

    [OdinSerialize] private Dictionary<PlayerCharacter.State, SoundState> sounds = null;

    private PlayerCharacter player;

    private PlayerCharacter.State actualState;

    private void Awake() {
        player = FindObjectOfType<PlayerCharacter>();
        player.ChangeStateEvent += OnSwitchState;

        actualState = player.ActualState;
        BeginState();
    }

    private void Update() {
        if(!sounds.ContainsKey(actualState)){
            return;
        }

        SoundState state = sounds[actualState];

        if(state.firstState){
            if(!state.beginSound.IsPlaying()){
                state.firstState = false;
                state.beginSound.ResetSound();
                state.continuousSound?.PlaySound();
            }
        }
    }

    public void OnSwitchState(){
        if(sounds.ContainsKey(actualState)){
            SoundState oldState = sounds[actualState];
            if(oldState.continuousSound != null){
                oldState.continuousSound.StopSound();
                oldState.continuousSound.ResetSound();
            }

            if(oldState.endSound != null){
                oldState.endSound.ResetSound();
                oldState.endSound.PlaySound();
            }
            
            oldState.endSound?.PlaySound();
        }

        actualState = player.ActualState;
        BeginState();
    }

    public void BeginState(){
        if(sounds.ContainsKey(actualState)){
            SoundState state = sounds[actualState];
            if(state.beginSound != null){
                state.beginSound.PlaySound();
                state.firstState = true;
            }
            else{
                state.continuousSound?.PlaySound();
                state.firstState = false;
            }
        }
    }
}
