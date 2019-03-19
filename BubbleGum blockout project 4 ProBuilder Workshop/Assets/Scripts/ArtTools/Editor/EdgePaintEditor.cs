using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EdgePaintEditor : EditorWindow
{
    private const float ERROR = 0.01f;
    private enum CreateEdgeState{WAIT, FIRST_POINT, SECOND_POINT, EDITION}
    private const string EDGE_TAG = "Edge";

    private bool displaySpheres = false;
    private float sphereSize = 0.2f;
    private float edgeLength = 0.05f;
    private float edgeHeight = 0.01f;
    private Material edgeMaterial = null;

    private GameObject actualObject = null;
    private MeshFilter actualMesh = null;
    private Edge actualCreatedMesh = null;
    private MeshRenderer actualCreatedMeshRenderer = null;
    private Vector3[] actualVertices = new Vector3[6];

    private List<Edge> edges = new List<Edge>();

    private Vector3[] points = new Vector3[2];
    private Vector3[] normals = new Vector3[2];
    private int[] triangles = new int[2];

    private CreateEdgeState state;

    private CreateEdgeState lastState;

    private bool needRepaint;
    private bool needRepaintEditor;

    private string errorMessage = "";

    private bool needInverse = false;

    [MenuItem("Bubble Gum/Edge Paint")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        EdgePaintEditor window = (EdgePaintEditor) EditorWindow.GetWindow(typeof(EdgePaintEditor), false, "Edge Creator");

        window.state = CreateEdgeState.WAIT;
        window.lastState = CreateEdgeState.WAIT;
        
        window.Show();
    }

    void OnFocus() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    private void OnInspectorUpdate()
    {
        if(Selection.activeGameObject != actualObject){
            Repaint();
        }

        if(lastState != state){
            lastState = state;
            Repaint();
            needRepaint = true;
        }

        if(needRepaintEditor){
            Repaint();
            needRepaintEditor = false;
        }

        if(Selection.activeGameObject == null){
            actualMesh = null;
            actualObject = null;
            state = CreateEdgeState.WAIT;
            return;
        }

        if(Selection.activeGameObject != actualObject){
            actualObject = Selection.activeObject as GameObject;
            actualMesh = actualObject?.GetComponent<MeshFilter>();

            if(actualMesh != null){
                edges.Clear();

                Transform t = actualObject.GetComponent<Transform>();
                
                for(int i = 0; i < t.childCount; i++){
                    Edge child = t.GetChild(i).GetComponent<Edge>();
                    if(child != null){
                        edges.Add(child);
                    }
                }
            }

            state = CreateEdgeState.WAIT;
            Repaint();
            return;
        }
    }

    private void OnSceneGUI(SceneView sceneView){
        OnSceneGUI();
        if(needRepaint){
            HandleUtility.Repaint();
            needRepaint = false;
        }
    }

    private void OnSceneGUI() {
        if(actualMesh == null){
            return;
        }

        Event guiEvent = Event.current;

        if(guiEvent.type == EventType.Layout){
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));          
            return;
        }

        if((state == CreateEdgeState.WAIT) || (state == CreateEdgeState.EDITION)){
            if((guiEvent.type == EventType.MouseUp) && (guiEvent.button == 0)){
                GameObject selection = MeshRaycast.PickGameObject(guiEvent.mousePosition);

                if(selection == null){
                    Selection.activeGameObject = null;
                    actualCreatedMesh = null;
                    actualCreatedMeshRenderer = null;
                }
                else{
                    Edge mesh = selection.GetComponent<Edge>();

                    if((mesh != null) && edges.Contains(mesh)){
                        ChangeSelection(mesh);   
                    }
                    else{
                        Selection.activeGameObject = selection;
                        actualCreatedMesh = null;
                        actualCreatedMeshRenderer = null;
                    }

                    state = CreateEdgeState.WAIT;
                    needRepaintEditor = true;
                }
            }

            if((guiEvent.type == EventType.KeyDown) && (guiEvent.keyCode == KeyCode.Space)){
                state = CreateEdgeState.FIRST_POINT;
            }
        }

        switch(state){
            case CreateEdgeState.FIRST_POINT:
                if((guiEvent.type == EventType.MouseUp) && (guiEvent.button == 0)){
                    Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                    RaycastHit hit;
                    if(MeshRaycast.IntersectRayMesh(mouseRay, actualMesh, out hit)){
                        points[0] = hit.point;
                        triangles[0] = hit.triangleIndex;
                        normals[0] = GetNormal(actualMesh, hit.triangleIndex);

                        DrawSphere(points[0]);
                        needRepaint = true;

                        errorMessage = null;

                        state = CreateEdgeState.SECOND_POINT;
                    }
                }
                break;
            case CreateEdgeState.SECOND_POINT:
                DrawSphere(points[0]);
                if((guiEvent.type == EventType.MouseUp) && (guiEvent.button == 0)){
                    Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                    RaycastHit hit;
                    if(MeshRaycast.IntersectRayMesh(mouseRay, actualMesh, out hit)){
                        if(hit.normal != normals[0]){
                            points[1] = hit.point;
                            triangles[1] = hit.triangleIndex;
                            normals[1] = GetNormal(actualMesh, hit.triangleIndex);

                            BuildMesh();

                            if(displaySpheres){
                                for(int i = 1; i < 2; i++){
                                    DrawSphere(points[i]);
                                }

                                needRepaint = true;
                            }

                            if(errorMessage == null){
                                state = CreateEdgeState.EDITION;
                            }
                            else{
                                state = CreateEdgeState.FIRST_POINT;
                            }
                        }
                    }
                }
                break;
            case CreateEdgeState.EDITION:
                if(displaySpheres){
                    for(int i = 0; i < 2; i++){
                        DrawSphere(points[i]);
                    }
                }
                

                if(needInverse || ((guiEvent.type == EventType.KeyDown) && (guiEvent.keyCode == KeyCode.C))){
                    needInverse = false;
                    if(actualCreatedMesh != null){
                        Undo.RecordObject(actualCreatedMesh.gameObject, "Inverse Mesh");
                        InvertHeight();

                        Vector3[] reversedVertices = new Vector3[6];
                        reversedVertices[0] = actualVertices[1];
                        reversedVertices[1] = actualVertices[0];
                        reversedVertices[2] = actualVertices[3];
                        reversedVertices[3] = actualVertices[2];
                        reversedVertices[4] = actualVertices[5];
                        reversedVertices[5] = actualVertices[4];

                        actualVertices = reversedVertices;
                        actualCreatedMesh.meshFilter.sharedMesh.vertices = actualVertices;
                    }
                }
                break;
        }
    }

    private void BuildMesh(){
        int[,] faces = new int[2,4];
        int[] meshTriangles = actualMesh.sharedMesh.triangles;
        Vector3[] meshVertices = actualMesh.sharedMesh.vertices;
        Vector3[] pointPositions = new Vector3[3];
        Transform meshTransform = actualMesh.GetComponent<Transform>();

        List<int> otherPoints = new List<int>();
        int[] possiblePoints = new int[]{0, 1, 2};

        for(int i = 0; i < 2; i++){
            for(int j = 0; j < 3; j++){
                faces[i,j] = meshTriangles[triangles[i] * 3 + j];
            }

            for(int j = 0; j < meshTriangles.Length; j+=3){
                int nbSamePoints = 0;
                otherPoints.Clear();
                otherPoints.InsertRange(0, possiblePoints);
                
                for(int k = 0; k < 3; k++){
                    for(int l = 0; l < 3; l++){
                        if(faces[i,k] == meshTriangles[j + l]){
                            nbSamePoints++;
                            otherPoints.Remove(l);
                        }
                    }

                    pointPositions[k] = meshVertices[meshTriangles[j + k]];
                }

                if(nbSamePoints == 2){
                    Vector3 n = GetNormal(meshTransform, pointPositions);
                    if(IsVectorEquals(n, normals[i])){
                        faces[i,3] = meshTriangles[j + otherPoints[0]];
                        continue;
                    }
                }
            }
        }

        int index = 0;
        int[] edgeIds = new int[4];

        for(int i = 0; i < 4; i++){
            for(int j = 0; j < 4; j++){
                Vector3 firstFace = meshTransform.TransformPoint(meshVertices[faces[0,i]]);
                Vector3 secondFace = meshTransform.TransformPoint(meshVertices[faces[1,j]]);
                if(firstFace == secondFace){
                    if(index >= 2){
                        index = 3;
                        break;
                    }
                    points[index] = meshTransform.TransformPoint(meshVertices[faces[0,i]]);
                    edgeIds[index] = i;
                    edgeIds[2 + index] = j; 
                    index++;
                }
            }

            if(index > 2){
                index = 3;
                break;
            }
        }

        if(index != 2){
            errorMessage = "Can't find the edge. Retry";
        }
        else{
            Mesh newMesh = new Mesh();
            Vector3[] vertices = new Vector3[6];

            GameObject newMeshObject = new GameObject();
            newMeshObject.name = "Edge " + (edges.Count + 1);
            newMeshObject.tag = "Edge";
            newMesh.name = newMeshObject.name + "_Mesh";
            Transform newMeshTransform = newMeshObject.GetComponent<Transform>();
            newMeshObject.AddComponent<MeshFilter>();
            actualCreatedMesh = newMeshObject.AddComponent<Edge>();
            actualCreatedMesh.meshFilter = actualCreatedMesh.GetComponent<MeshFilter>();
            actualCreatedMesh.meshFilter.sharedMesh = newMesh;
            actualCreatedMeshRenderer = newMeshObject.AddComponent<MeshRenderer>();
            actualCreatedMeshRenderer.sharedMaterial = edgeMaterial;

            newMeshTransform.SetParent(meshTransform);
            newMeshTransform.position = ((points[0] - points[1]) / 2f) + points[1];
            newMeshTransform.localRotation = Quaternion.identity;

            vertices[0] = newMeshTransform.InverseTransformPoint(points[0]);
            vertices[1] = newMeshTransform.InverseTransformPoint(points[1]);

            Vector3 edge = vertices[1] - vertices[0];

            bool ifirstAngle = true;
            bool jfirstAngle = true;
            int iFirstPoint = 0;
            int jFirstPoint = 0;

            Vector3[,] transformedVertices = new Vector3[2,4];

            for(int i = 0; i < 2; i++){
                for(int j = 0; j < 4; j++){
                    transformedVertices[i,j] = meshTransform.TransformPoint(meshVertices[faces[i,j]]);
                    transformedVertices[i,j] = newMeshTransform.InverseTransformPoint(transformedVertices[i,j]);
                }
            }

            for(int i = 0; i < 4; i++){
                if((edgeIds[0] != i) && (edgeIds[1] != i)){
                    
                    if(ifirstAngle){
                        iFirstPoint = i;
                        ifirstAngle = false;
                    }
                    else{
                        Vector3 p1 = transformedVertices[0,iFirstPoint];
                        Vector3 p2 = transformedVertices[0,i];
                        float angle1 = Vector3.Angle(edge, p1 - vertices[1]);
                        float angle2 = Vector3.Angle(edge, p2 - vertices[1]);

                        if(angle1 > angle2){
                            vertices[2] = p1;
                            vertices[3] = p2;
                        }
                        else{
                            vertices[2] = p2;
                            vertices[3] = p1;
                        }
                    }
                    
                    
                }

                if((edgeIds[2] != i) && (edgeIds[3] != i)){
                    if(jfirstAngle){
                        jFirstPoint = i;
                        jfirstAngle = false;
                    }
                    else{
                        Vector3 p1 = transformedVertices[1,jFirstPoint];
                        Vector3 p2 = transformedVertices[1,i];
                        float angle1 = Vector3.Angle(edge, p1 - vertices[1]);
                        float angle2 = Vector3.Angle(edge, p2 - vertices[1]);

                        if(angle1 > angle2){
                            vertices[4] = p1;
                            vertices[5] = p2;
                        }
                        else{
                            vertices[4] = p2;
                            vertices[5] = p1;
                        }
                    }
                    
                    
                }
            }

            RecalculateLength(vertices);

            Vector3 actualHeightDirection = Vector3.Cross(vertices[5] - vertices[3], vertices[2] - vertices[3]).normalized;

            for(int i = 0; i < 6; i++){
                vertices[i] += edgeHeight * actualHeightDirection;
                actualVertices[i] = vertices[i];
            }

            newMesh.vertices = vertices;
            newMesh.triangles = SetTriangles();
            newMesh.uv = SetUvs();

            edges.Add(actualCreatedMesh);
            Undo.RegisterCreatedObjectUndo(actualObject, "Create Edge");

            actualCreatedMesh.SaveMesh();

            // DestroyImmediate(newMeshObject);
        }
    }

    private void OnGUI() {
        if(errorMessage != null){
            EditorGUILayout.LabelField(errorMessage);
        }

        if(actualObject == null){
            EditorGUILayout.LabelField("Select a Mesh");
            return;
        }

        if(actualMesh == null){
            EditorGUILayout.LabelField("This Object don't have Mesh");
            return;
        }

        switch(state){
            case CreateEdgeState.FIRST_POINT:
                EditorGUILayout.LabelField("Choose the first point");
                break;
            case CreateEdgeState.SECOND_POINT:
                EditorGUILayout.LabelField("Choose the second point");
                break;
        }

        if(state == CreateEdgeState.WAIT){
            if(GUILayout.Button("Add Edge")){
                state = CreateEdgeState.FIRST_POINT;
            }
        }
        else if(state == CreateEdgeState.EDITION){
            if(GUILayout.Button("Remove Edge")){
                state = CreateEdgeState.WAIT;
                errorMessage = null;

                if(actualCreatedMesh != null){
                    if(edges.Contains(actualCreatedMesh)){
                        edges.Remove(actualCreatedMesh);
                    }

                    Undo.RecordObject(actualCreatedMesh.gameObject, "Remove Edge");

                    DestroyImmediate(actualCreatedMesh.gameObject);
                    actualCreatedMesh = null;
                    actualCreatedMeshRenderer = null;
                }

                for(int i = 0; i < edges.Count; i++){
                    edges[i].name = "Edge " + (i + 1);
                }
            }
            if(GUILayout.Button("Add Edge")){
                state = CreateEdgeState.FIRST_POINT;
            }
        }
        else{
            if(GUILayout.Button("Cancel")){
                state = CreateEdgeState.WAIT;
                errorMessage = null;

                if(actualCreatedMesh != null){
                    DestroyImmediate(actualCreatedMesh.gameObject);
                }
            }
        }

        GUI.enabled = (state == CreateEdgeState.WAIT);

        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Edge Parameters");

        float oldLength = edgeLength;
        edgeLength = EditorGUILayout.FloatField("Edge Length", edgeLength);

        float oldHeight = edgeHeight;
        edgeHeight = EditorGUILayout.FloatField("Edge Height", edgeHeight);

        if(oldLength != edgeLength){
            if(edgeLength <= 0f){
                edgeLength = oldLength;
            }
            else if(state == CreateEdgeState.EDITION){
                RecalculateLength(actualVertices);
                actualCreatedMesh.meshFilter.sharedMesh.vertices = actualVertices;
                actualCreatedMesh.SaveMesh();
                needRepaint = true;
            }
        }

        if(oldHeight != edgeHeight){
            if(edgeHeight <= 0f){
                edgeHeight = oldHeight;
            }
            else if(state == CreateEdgeState.EDITION){
                RecalculateHeight();
                needRepaint = true;
            }
        }

        if(state != CreateEdgeState.EDITION){
            edgeMaterial = EditorGUILayout.ObjectField("Edge Material", edgeMaterial, typeof(Material), false) as Material;
        }
        else{
            Material oldMaterial = edgeMaterial;
            edgeMaterial = EditorGUILayout.ObjectField("Edge Material", edgeMaterial, typeof(Material), false) as Material;
            if(oldMaterial != edgeMaterial){
                actualCreatedMeshRenderer.sharedMaterial = edgeMaterial;
                needRepaint = true;
            }
        }

        GUI.enabled = (state == CreateEdgeState.EDITION);

        if(GUILayout.Button("Inverse faces")){
            needInverse = true;
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shortcuts");
        EditorGUILayout.LabelField("Space : Add Edge");
        EditorGUILayout.LabelField("C : Inverse Edge");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Edges :");

        GUI.enabled = ((state == CreateEdgeState.WAIT) || (state == CreateEdgeState.EDITION));

        foreach(Edge mf in edges){
            if(actualCreatedMesh == mf){
                GUI.enabled = false;
                GUILayout.Button(mf.name);
                GUI.enabled = ((state == CreateEdgeState.WAIT) || (state == CreateEdgeState.EDITION));
                continue;
            }


            if(GUILayout.Button(mf.name)){
                ChangeSelection(mf);
            }
        }

        EditorGUILayout.Space();

        if(GUILayout.Button("Remove all edges (Warning it can be dangerous)")){
            state = CreateEdgeState.WAIT;
            errorMessage = null;

            Edge[] removedEdges = edges.ToArray();
            Undo.RecordObjects(removedEdges, "Remove All Edges");
            edges.Clear();

            for(int i = 0; i < removedEdges.Length; i++){
                DestroyImmediate(removedEdges[i].gameObject);
            }

            actualCreatedMesh = null;
            actualCreatedMeshRenderer = null;
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("DEBUG");

        if(state != CreateEdgeState.EDITION){
            displaySpheres = EditorGUILayout.Toggle("Display spheres", displaySpheres);
            sphereSize = EditorGUILayout.FloatField("SphereSize", sphereSize);
        }
        else{
            bool oldDisplay = displaySpheres;
            displaySpheres = EditorGUILayout.Toggle("Display spheres", displaySpheres);
            if(displaySpheres != oldDisplay){
                needRepaint = true;
            }

            float oldSize = sphereSize;
            sphereSize = EditorGUILayout.FloatField("SphereSize", sphereSize);
            if(oldSize != sphereSize){
                needRepaint = true;
            }
        }
    }

    private void DrawSphere(Vector3 position){
        DrawSphere(position, Color.white, sphereSize);
    }

    private void DrawSphere(Vector3 position, float size){
        DrawSphere(position, Color.white, size);
    }

    private void DrawSphere(Vector3 position, Color color){
        DrawSphere(position, color, sphereSize);
    }

    private void DrawSphere(Vector3 position, Color color, float size){
        Color a = Handles.color;
        Handles.color = color;
        Handles.SphereHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
        Handles.color = a;
    }

    private void DrawLine(Vector3 position, Vector3 direction, float lenght){
        DrawLine(position, direction, lenght, Color.white);
    }

    private void DrawLine(Vector3 position, Vector3 direction, float lenght, Color color){
        Color a = Handles.color;
        Handles.color = color;
        Handles.DrawAAPolyLine(new Vector3[]{position, position + direction * lenght});
        Handles.color = a;
    }

    private bool IsVectorEquals(Vector3 v1, Vector3 v2){
        bool result = (Mathf.Abs(v1.x - v2.x) < ERROR);
        result = result && (Mathf.Abs(v1.y - v2.y) < ERROR);
        result = result && (Mathf.Abs(v1.z - v2.z) < ERROR);

        return result;
    }

    private Vector3 GetNormal(MeshFilter meshFilter, int triangleIndex){
        Transform meshTransform = meshFilter.GetComponent<Transform>();
        int[] triangles = meshFilter.sharedMesh.triangles;
        Vector3[] points = new Vector3[3];
        points[0] = meshFilter.sharedMesh.vertices[triangles[triangleIndex * 3]];
        points[1] = meshFilter.sharedMesh.vertices[triangles[triangleIndex * 3 + 1]];
        points[2] = meshFilter.sharedMesh.vertices[triangles[triangleIndex * 3 + 2]];
        return GetNormal(meshTransform, points);
    }

    private Vector3 GetNormal(Transform meshTransform, Vector3[] points){
        return GetNormal(meshTransform, points[0], points[1], points[2]);
    }

    private Vector3 GetNormal(Transform meshTransform, Vector3 p1, Vector3 p2, Vector3 p3){
        Vector3 tp1 = meshTransform.TransformPoint(p1);
        Vector3 tp2 = meshTransform.TransformPoint(p2);
        Vector3 tp3 = meshTransform.TransformPoint(p3);
        return Vector3.Cross(tp2 - tp1, tp3 - tp1).normalized;
    }

    private int[] SetTriangles(){
        int[] result = new int[12];
        result[0] = 0; result[1] = 2; result[2] = 3;
        result[3] = 0; result[4] = 3; result[5] = 1;
        result[6] = 0; result[7] = 1; result[8] = 5;
        result[9] = 0; result[10] = 5; result[11] = 4;

        return result;
    }

    private Vector2[] SetUvs(){
        Vector2[] result = new Vector2[6];
        result[3] = new Vector2(0f, 0.45f);
        result[1] = new Vector2(0f, 0.5f);
        result[5] = new Vector2(0f, 0.55f);
        result[2] = new Vector2(1f, 0.45f);
        result[0] = new Vector2(1f, 0.5f);
        result[4] = new Vector2(1f, 0.55f);

        return result;
    }

    private void RecalculateHeight(){
        Vector3 direction = ((actualVertices[1] - actualVertices[0]) / 2f + actualVertices[0]);
        Vector3 normalizedDirection = direction.normalized;
        for(int i = 0; i < 6; i++){
            actualVertices[i] = actualVertices[i] - direction + normalizedDirection * edgeHeight;
        }

        actualCreatedMesh.meshFilter.sharedMesh.vertices = actualVertices;
        actualCreatedMesh.SaveMesh();
    }

    private void InvertHeight(){
        Vector3 direction = ((actualVertices[1] - actualVertices[0]) / 2f + actualVertices[0]);
        for(int i = 0; i < 6; i++){
            actualVertices[i] = actualVertices[i] - 2f * direction;
        }

        actualCreatedMesh.meshFilter.sharedMesh.vertices = actualVertices;
        actualCreatedMesh.SaveMesh();
    }

    private void RecalculateLength(Vector3[] vertices){
        vertices[3] = vertices[1] + (vertices[3] - vertices[1]).normalized * edgeLength;
        vertices[2] = vertices[0] + (vertices[2] - vertices[0]).normalized * edgeLength;
        vertices[4] = vertices[0] + (vertices[4] - vertices[0]).normalized * edgeLength;
        vertices[5] = vertices[1] + (vertices[5] - vertices[1]).normalized * edgeLength;
    }

    private void ChangeSelection(Edge newSelection){
        actualCreatedMesh = newSelection;
        actualCreatedMeshRenderer = newSelection.GetComponent<MeshRenderer>();
        actualVertices = actualCreatedMesh.meshFilter.sharedMesh.vertices;
        edgeLength = Vector3.Distance(actualVertices[0], actualVertices[2]);
        edgeMaterial = actualCreatedMeshRenderer.sharedMaterial;
        needRepaint = true;

        state = CreateEdgeState.EDITION;
    }
}
