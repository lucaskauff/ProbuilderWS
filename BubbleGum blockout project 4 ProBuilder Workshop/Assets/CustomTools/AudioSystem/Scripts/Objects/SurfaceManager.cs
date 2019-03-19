using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem{
	public class SurfaceManager : MonoBehaviour {	
		

		[Header("Songs")]
		public ClipInformations defaultClip;
		public ClipInformations[] clips;

		[Header("Surfaces")]
		public PhysicMaterial[] materials;
	}
}