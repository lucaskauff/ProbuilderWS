using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Edge : MonoBehaviour
{

    #if UNITY_EDITOR

        [SerializeField, HideInInspector] public MeshFilter meshFilter;
        [SerializeField, HideInInspector] private Vector3[] vertices;
        [SerializeField, HideInInspector] private int[] triangles;
        [SerializeField, HideInInspector] private Vector2[] uv;

        private void Awake()
        {
            if(meshFilter == null){
                return;
            }
            
            if(meshFilter.sharedMesh == null){
                Mesh mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uv;

                meshFilter.sharedMesh = mesh;
            }
        }

        public void SaveMesh(){
            vertices = meshFilter.sharedMesh.vertices;
            triangles = meshFilter.sharedMesh.triangles;
            uv = meshFilter.sharedMesh.uv;
        }

        private void Reset() {
            hideFlags = HideFlags.DontSaveInBuild;
        }

    #endif
}
