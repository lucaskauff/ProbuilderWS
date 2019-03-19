using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
	public class ProbabilityNearSound : SoundPlayer {

		[Header("Probability")]
		[SerializeField, Range(0f, 1f)] private float soundProbability = 0f;

		[Tooltip("Min time between two songs")]
		[SerializeField] private float minTime = 0f;

		private float time;

		protected override void Awake() {
			base.Awake();
			time = minTime;
		}

		private void OnTriggerExit(Collider other) {
			if(other.gameObject.tag != "Player"){
				return;
			}

			if(time < minTime){
				time += Time.deltaTime;
				return;
			}

			float rand = Random.Range(0f, 1f);
			if(rand < soundProbability){			
				sound.PlaySound();
				time = 0f;
			}
		}
	}
}