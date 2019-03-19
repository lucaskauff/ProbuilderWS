using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
	public struct JoyconMotions{
		public bool useMotions;

		[Tooltip("Motion valid if over the value")]
		public Vector3 motionAccuracy;

		[Tooltip("Time to be considered in motion")]
		public float timeBeforeMotion;

		[Tooltip("time to no longer be considered in boost")]
		public float timeAfterMotion;

		public bool IsLeftMotion {get; private set;}
		public bool IsRightMotion {get; private set;}
		private float leftTiming;
		private float rightTiming;

		// public JoyconMotions(bool useMotions, Vector3 motionAccuracy, float timeBeforeMotion, float timeAfterMotion){
		// 	this.useMotions = useMotions;
		// 	this.motionAccuracy = motionAccuracy;
		// 	this.timeBeforeMotion = timeBeforeMotion;
		// 	this.timeAfterMotion = timeAfterMotion;

		// 	this.IsLeftMotion = false;
		// 	this.IsRightMotion = false;
		// 	this.leftTiming = 0f;
		// 	this.rightTiming = 0f;
		// }

		public void UpdateMotions(Vector3 leftAccel, Vector3 rightAccel, float deltaTime){
			bool leftOver = IsAccelerationOver(motionAccuracy, leftAccel);
			bool rightOver = IsAccelerationOver(motionAccuracy, rightAccel);

			if(IsLeftMotion){
				if(leftOver){
					leftTiming = 0f;
				}
				else{
					leftTiming += deltaTime;
					if(leftTiming > timeAfterMotion){
						IsLeftMotion = false;
						leftTiming = 0f;
					}
				}
			}
			else{
				if(!leftOver){
					leftTiming = 0f;
				}
				else{
					leftTiming += deltaTime;
					if(leftTiming > timeBeforeMotion){
						IsLeftMotion = true;
						leftTiming = 0f;
					}
				}
			}

			if(IsRightMotion){
				if(rightOver){
					rightTiming = 0f;
				}
				else{
					rightTiming += deltaTime;
					if(rightTiming > timeAfterMotion){
						IsRightMotion = false;
						rightTiming = 0f;
					}
				}
			}
			else{
				if(!rightOver){
					rightTiming = 0f;
				}
				else{
					rightTiming += deltaTime;
					if(rightTiming > timeBeforeMotion){
						IsRightMotion = true;
						rightTiming = 0f;
					}
				}
			}
		}

		public bool InMotions(){
			return IsLeftMotion && IsRightMotion;
		}

		private static bool IsAccelerationOver(Vector3 minimum, Vector3 acceleration){
			if(minimum.x > Mathf.Abs(acceleration.x)){
				return false;
			}

			if(minimum.y > Mathf.Abs(acceleration.y)){
				return false;
			}

			if(minimum.z > Mathf.Abs(acceleration.z)){
				return false;
			}

			return true;
		}
	}