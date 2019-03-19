using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintingSystem
{

	public class GroundPainter : MonoBehaviour {

		[SerializeField] private bool usePainting = false;

		[Header("Trace")]
		[SerializeField] private PaintingLineMesh tracePrefab = null;

		[Header("Raycast")]
		[SerializeField] private Transform leftRaycastPosition = null;
		[SerializeField] private Transform centerRaycastPosition = null;
		[SerializeField] private Transform rightRaycastPosition = null;
		[SerializeField] private float raycastLength = 0f;
		[SerializeField] private LayerMask ignoredLayers = 0;

		[Header("Debug")]
		[SerializeField] private bool debug =false;

		private bool isActivated;

		private Transform actualParent;

		private List<PaintingLineMesh> lines;

		private void Awake() {
			Desactivate();
			lines = new List<PaintingLineMesh>();
		}
		
		void Update () {
			if(!usePainting){
				return;
			}

			if(debug){
				Debug.DrawLine(leftRaycastPosition.position, leftRaycastPosition.position - leftRaycastPosition.up * raycastLength, Color.red);
				Debug.DrawLine(rightRaycastPosition.position, rightRaycastPosition.position - rightRaycastPosition.up * raycastLength, Color.red);
			}

			PaintingLineMesh lastLine = null;
			if(lines.Count > 0){
				lastLine = lines[lines.Count - 1];
			}

			RaycastHit leftHit;
			RaycastHit centerHit;
			RaycastHit rightHit;

			bool ray = Physics.Raycast(leftRaycastPosition.position, -leftRaycastPosition.up, out leftHit, raycastLength, ~ignoredLayers);
			
			centerHit = leftHit;
			rightHit = leftHit;
			
			ray = ray && Physics.Raycast(centerRaycastPosition.position, -centerRaycastPosition.up, out centerHit, raycastLength, ~ignoredLayers);
			ray = ray && Physics.Raycast(rightRaycastPosition.position, -rightRaycastPosition.up, out rightHit, raycastLength, ~ignoredLayers);

			if(ray){
				Transform centerTransform = centerHit.collider.GetComponent<Transform>();
				bool needActivated = (!isActivated || (lastLine == null));
				if(needActivated){
					Activate(centerTransform);
					return;
				}

				bool changeParent = (centerTransform != actualParent) && !(centerTransform.gameObject.isStatic && actualParent.gameObject.isStatic);
				Vector3 leftPoint = leftHit.point + leftHit.normal * 0.001f;
				Vector3 centerPoint = centerHit.point + centerHit.normal * 0.001f;
				Vector3 rightPoint = rightHit.point + rightHit.normal * 0.001f;

				if(changeParent){
					lastLine.AddPoints(leftPoint, centerPoint, rightPoint);
					CreateNewLine(centerTransform);
				}

				lastLine.AddPoints(leftPoint, centerPoint, rightPoint);
			}
			else{
				Desactivate();
			}			
		}

		public void ClearTraces(){
			PaintingLineMesh[] lineArray = lines.ToArray();
			foreach (PaintingLineMesh l in lineArray)
			{
				Destroy(l.gameObject);
			}

			lines.Clear();
			Desactivate();
		}
		
		public void Activate(Transform parent){
			if(isActivated || !usePainting){
				return;
			}
			
			isActivated = true;

			CreateNewLine(parent);
		}

		public void Desactivate(){
			isActivated = false;
			actualParent = null;
		}

		private void CreateNewLine(Transform parent){
			if(!isActivated){
				return;
			}

			PaintingLineMesh lastLine = Instantiate<PaintingLineMesh>(tracePrefab);
			Transform actualLineTransform = lastLine.GetComponent<Transform>();
			actualLineTransform.parent = parent;
			actualParent = parent;

			lines.Add(lastLine);
		}

		private bool IsRaycastDefined(){
			if(leftRaycastPosition == null){
				return false;
			}

			if(rightRaycastPosition == null){
				return false;
			}

			if(centerRaycastPosition == null){
				return false;
			}

			return true;
		}

	#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if(!IsRaycastDefined()){
				return;
			}

			bool notActive = (Selection.activeObject != gameObject);
			notActive = notActive && (Selection.activeObject != leftRaycastPosition.gameObject);
			notActive = notActive && (Selection.activeObject != centerRaycastPosition.gameObject);
			notActive = notActive && (Selection.activeObject != rightRaycastPosition.gameObject);

			if(notActive){
				return;
			}

			Gizmos.color = Color.green;
			DrawRayGizmos(leftRaycastPosition);
			DrawRayGizmos(centerRaycastPosition);
			DrawRayGizmos(rightRaycastPosition);
		}

		private void DrawRayGizmos(Transform rayPos){
			Gizmos.DrawSphere(rayPos.position, 0.04f);
			Gizmos.DrawLine(rayPos.position, rayPos.position + -raycastLength * rayPos.up);
		}
	#endif
	}
}