using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingSystem
{

	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class PaintingLineMesh : MonoBehaviour {

		private Mesh mesh;
		private MeshFilter meshFilter;
		private Transform selfTransform;

		private List<Vector3> vertices;
		private List<int> triangles;
		// private List<Vector3> normals;
		private List<Vector2> uvs;
		private List<float> distances;

		private Vector3 lastCenterPoint;
		private Vector3 lastLeftPoint;
		private Vector3 lastRightPoint;

		private bool firstPoint;

		private float totalDistance;

		private void Awake() {
			selfTransform = GetComponent<Transform>();
			meshFilter = GetComponent<MeshFilter>();
			mesh = new Mesh();
			meshFilter.mesh = mesh;

			vertices = new List<Vector3>();
			triangles = new List<int>();
			// normals = new List<Vector3>();
			uvs = new List<Vector2>();
			distances = new List<float>();

			firstPoint = true;
		}

		public void AddPoints(Vector3 left, Vector3 center, Vector3 right){
			Vector3 localLeft = selfTransform.InverseTransformPoint(left);
			Vector3 localCenter = selfTransform.InverseTransformPoint(center);
			Vector3 localRight = selfTransform.InverseTransformPoint(right);
			
			if(firstPoint){
				lastLeftPoint = localLeft;
				lastCenterPoint = localCenter;
				lastRightPoint = localRight;

				totalDistance = 0f;
				distances.Add(0f);
				firstPoint = false;
				return;
			}

			float distleft = Vector3.Distance(lastLeftPoint, localLeft);
			float distCenter = Vector3.Distance(lastCenterPoint, localCenter);
			float distRight = Vector3.Distance(lastRightPoint, localRight);
			

			if((distCenter < 0.1f) && (distleft < 0.1f) && (distRight < 0.1f)){
				return;
			}

			int actualPointId = vertices.Count / 2;

			if(actualPointId == 0){
				AddPoint(lastLeftPoint);
				AddPoint(lastRightPoint);
				AddUvs();

				actualPointId = 1;
			}

			AddPoint(localLeft);
			AddPoint(localRight);

			distances.Add(distCenter);
			totalDistance += distCenter;

			AddUvs();
			AddTriangles(actualPointId); 
			
			lastLeftPoint = localLeft;
			lastCenterPoint = localCenter;
			lastRightPoint = localRight;

			mesh.SetVertices(vertices);
			mesh.SetUVs(0, uvs);
			mesh.SetTriangles(triangles, 0);

			mesh.RecalculateBounds();

			// mesh.vertices = vertices.ToArray();
			// mesh.uv = uvs.ToArray();
			// mesh.triangles = triangles.ToArray();

			// mesh.RecalculateNormals();
		}

		private void AddTriangles(int id){
			triangles.Add(2 * (id - 1)); 
			triangles.Add(2 * id); 
			triangles.Add((2 * id) - 1);

			triangles.Add(2 * (id - 1)); 
			triangles.Add((2 * id) - 1);
			triangles.Add(2 * id); 

			triangles.Add((2 * id) + 1); 
			triangles.Add((2 * id) - 1); 
			triangles.Add(2 * id);

			triangles.Add((2 * id) + 1); 
			triangles.Add(2 * id);
			triangles.Add((2 * id) - 1); 
		}

		private void AddPoint(Vector3 point){
			vertices.Add(point);
			// Vector3 rotatedPoint = (Quaternion.Inverse(rotation) * point) * scale;
			// uvs.Add(new Vector2(point.x, point.z));
		}

		private void AddUvs(){
			float v = 1 - Mathf.Abs(2 * totalDistance - 1);
			Vector2 pos1 = new Vector2(0, v);
			Vector2 pos2 = new Vector2(1, v);

			uvs.Add(pos1);
			uvs.Add(pos2);
		}

		// private void MakeUvs(){

		// 	// float actualDistance = 0f;
		// 	// // float inverseScale = (1f / scale);
		// 	// for(int i = 0; i < vertices.Count; i += 2){
		// 	// 	actualDistance += distances[i / 2];
		// 	// 	float completion = actualDistance/* totalDistance*/;
		// 	// 	float v = 1 - Mathf.Abs(2 * completion - 1);
		// 	// 	Vector2 pos1 = new Vector2(0, v/* * totalDistance*/);
		// 	// 	Vector2 pos2 = new Vector2(1, v/* * totalDistance*/);

		// 	// 	if(i < uvs.Count){
		// 	// 		uvs[i] = pos1;
		// 	// 		uvs[i + 1] = pos2;
		// 	// 	}
		// 	// 	else{
		// 	// 		uvs.Add(pos1);
		// 	// 		uvs.Add(pos2);
		// 	// 	}
		// 	// }

		// 	mesh.SetUVs(0, uvs);
		// }

	}
}