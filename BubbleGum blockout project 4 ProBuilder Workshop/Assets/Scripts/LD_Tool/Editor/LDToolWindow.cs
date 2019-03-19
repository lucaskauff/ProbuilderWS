using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LDToolWindow : EditorWindow
{
    private struct SelectedBloc{
        public GameObject reference;
        public Transform transform;
        public GameObject roof;
        public GameObject ceil;

        public bool canCeiling;
        public bool canFloor;

        public static readonly SelectedBloc Empty = new SelectedBloc();
    }

    private struct LastProperties{
        public float rotation;
        public bool hasCeiling;
        public bool hasFloor;

        public static readonly LastProperties Empty = new LastProperties();
    }

    private LDToolReferences references;

    private GameObject actualSelection;
    private GameObject correctSelection;
    private SelectedBloc selectedBloc;

    private Transform parent;
    private GameObject objectToPlace;
    private Transform inPlacingObject;

    private bool needRepaint;

    private GUIStyle titleStyle;
    private Texture upTexture;
    private Texture downTexture;

    private LastProperties lastProperties = LastProperties.Empty;

    private Vector2 scrollPos;


    [MenuItem("Bubble Gum/LD Tool")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        LDToolWindow window = (LDToolWindow) EditorWindow.GetWindow(typeof(LDToolWindow), false, "LD Tool");

        window.Show();
    }

    private void Awake() {
        titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;

        upTexture = AssetDatabase.LoadAssetAtPath("Assets/AmplifyShaderEditor/Plugins/EditorResources/UI/Nodes/PreviewOn.png", typeof(Texture)) as Texture;
        downTexture = AssetDatabase.LoadAssetAtPath("Assets/AmplifyShaderEditor/Plugins/EditorResources/UI/Nodes/PreviewOff.png", typeof(Texture)) as Texture;
    }

    void OnFocus() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView){
        Event guiEvent = Event.current;

        if(inPlacingObject != null){
            if(guiEvent.type == EventType.Layout){
                int control = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(control);
                GUIUtility.keyboardControl = control;
                return;
            }

            if(guiEvent.type == EventType.MouseMove){
                MoveInPlacingObject(sceneView, guiEvent);
            }

            if(guiEvent.type == EventType.KeyDown){
                if(guiEvent.keyCode == KeyCode.KeypadEnter){
                    RemoveObjectToPlace();
                    needRepaint = true;
                }
            }

            if(guiEvent.type == EventType.MouseDown){
                if(guiEvent.button == 0){
                    inPlacingObject.gameObject.hideFlags = HideFlags.None;
                    inPlacingObject.parent = parent;
                    Undo.RegisterCreatedObjectUndo(inPlacingObject.gameObject, "Create Block");
                    inPlacingObject = null;
                    ChangeObjectToPlace(objectToPlace);
                }
            }
        }

        if((selectedBloc.reference != null) && (selectedBloc.transform != null)){
            if(guiEvent.type == EventType.KeyDown){
                if(guiEvent.keyCode == KeyCode.KeypadMinus){
                    ToggleRoof();
                }

                if(guiEvent.keyCode == KeyCode.KeypadPlus){
                    ToggleCeil();
                }

                if(guiEvent.keyCode == KeyCode.KeypadPeriod){
                    RotateObject(correctSelection.GetComponent<Transform>());
                }
            }

            Vector3 pos = references.step / 2f;
            pos.z *= -1;
            Handles.DrawWireCube(selectedBloc.transform.position + pos, references.step);
        }

        if(parent != null){
            if(guiEvent.type == EventType.KeyDown){
                if((guiEvent.keyCode >= KeyCode.Keypad0) && (guiEvent.keyCode <= KeyCode.Keypad9)){
                    int value = guiEvent.keyCode - KeyCode.Keypad0;
                    if(objectToPlace == references.keys[value]){
                        RemoveObjectToPlace();
                    }
                    else{
                        ChangeObjectToPlace(value);
                        MoveInPlacingObject(sceneView, guiEvent);
                    }
                    needRepaint = true;
                }
            }
        }
    }

    private void OnInspectorUpdate()
    {
        if(inPlacingObject != null){
            SceneView.FocusWindowIfItsOpen(typeof(SceneView));         
        }

        if(needRepaint){
            Repaint();
            needRepaint = false;
        }

        if((references == null) || (references.keys == null)){
            return;
        }

        if(actualSelection != Selection.activeGameObject){
            if((inPlacingObject != null) && (Selection.activeObject != inPlacingObject.gameObject)){
                RemoveObjectToPlace();
            }

            actualSelection = Selection.activeGameObject;
            
            bool isGoodPrefab = false;

            if(actualSelection != null){
                PrefabAssetType type = PrefabUtility.GetPrefabAssetType(actualSelection);
                if((type == PrefabAssetType.Regular)){
                    GameObject root =  PrefabUtility.GetOutermostPrefabInstanceRoot(actualSelection);
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(root);

                    if(references.keys.Contains(prefab)){
                        correctSelection = root;
                        selectedBloc = new SelectedBloc();
                        selectedBloc.reference = references.keys[ArrayUtility.IndexOf(references.keys, prefab)];

                        selectedBloc.transform = root.GetComponent<Transform>();
                        selectedBloc.canFloor = (references.ldBlocks[selectedBloc.reference].floorPrefab != null);
                        selectedBloc.canCeiling = (references.ldBlocks[selectedBloc.reference].ceilingPrefab != null);

                        Transform visualChild = selectedBloc.transform.GetChild(0);

                        for(int i = 0; i < visualChild.childCount; i++){
                            Transform child = visualChild.GetChild(i);

                            GameObject childPref = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                            if(childPref == null){
                                continue;
                            }

                            if(references.ldBlocks[prefab].ceilingPrefab.Equals(childPref)){
                                selectedBloc.roof = child.gameObject;
                            }
                            
                            if(references.ldBlocks[prefab].floorPrefab.Equals(childPref)){
                                selectedBloc.ceil = child.gameObject;
                            }
                        }
                        
                        isGoodPrefab = true;
                    }
                    else{
                        selectedBloc = SelectedBloc.Empty;
                        correctSelection = null;
                    }
                }
                else{
                    selectedBloc = SelectedBloc.Empty;
                    correctSelection = null;
                }
            }
            else{
                selectedBloc = SelectedBloc.Empty;
                correctSelection = null;
            }

            if(!isGoodPrefab){
                correctSelection = null;
                selectedBloc = SelectedBloc.Empty;
            }

            Repaint();
        }

        if(parent != null){

        }
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scriptable Object", GUILayout.Width(120f));
            LDToolReferences oldReferences = references;
            references = EditorGUILayout.ObjectField(references, typeof(LDToolReferences), false) as LDToolReferences;
            if((oldReferences != references) || GUILayout.Button("Reload", GUILayout.Width(150f))){
                LoadScriptable();
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if((references == null) || (references.keys == null)){
            EditorGUILayout.LabelField("No Scriptable Load");
            return;
        }

        EditorGUILayout.LabelField("Selection Property", EditorStyles.boldLabel);

        if(correctSelection == null){
            EditorGUILayout.LabelField("No block selected");
            GUI.enabled = false;

            EditorGUILayout.Toggle("Ceiling", lastProperties.hasFloor);
            EditorGUILayout.Toggle("Floor", lastProperties.hasCeiling);
            GUI.enabled = true;
        }
        else{
            EditorGUILayout.LabelField("Prefab : " + selectedBloc.reference.name);

            bool hasCeiling = (selectedBloc.roof != null);
            bool hasFloor = (selectedBloc.ceil != null);

            GUI.enabled = (selectedBloc.canCeiling);

            if(EditorGUILayout.Toggle("Ceiling", hasCeiling) != hasCeiling){
                ToggleRoof();
            }

            GUI.enabled = (selectedBloc.canFloor);

            if(EditorGUILayout.Toggle("Floor", hasFloor) != hasFloor){
                ToggleCeil();
            }

            GUI.enabled = true;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Spawn Blocs", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spawn Parent", GUILayout.Width(120f));
            parent = EditorGUILayout.ObjectField(parent, typeof(Transform), true) as Transform;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step", GUILayout.Width(120f));
            Vector3 newStep = EditorGUILayout.Vector3Field("", references.step);
            if(newStep.x > 0f){
                references.step.x = newStep.x;
            }

            if(newStep.y > 0f){
                references.step.y = newStep.y;
            }

            if(newStep.z > 0f){
                references.step.z = newStep.z;
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Offset", GUILayout.Width(120f));
            references.offset = EditorGUILayout.Vector3Field("", references.offset);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shortcuts", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Enter : Cancel");
        EditorGUILayout.LabelField("Left Click : Place");
        EditorGUILayout.LabelField("0-9 : Add corresponding block");
        EditorGUILayout.LabelField("+ : Toggle Ceiling");
        EditorGUILayout.LabelField("- : Toggle Floor");
        EditorGUILayout.LabelField("Dot : Rotate");

        EditorGUILayout.Space();

        if(parent == null){
            EditorGUILayout.LabelField("Set Parent first", titleStyle);
            GUI.enabled = false;
        }

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
    
        for(int i = 0; i < references.keys.Length; i++){
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i + " : ", GUILayout.Width(30f));
                bool isSelected = (objectToPlace == references.keys[i]);
                if(GUILayout.Toggle(isSelected, "Add " + references.keys[i].name, "Button") != isSelected){
                    if(isSelected){
                        RemoveObjectToPlace();
                    }
                    else{
                        ChangeObjectToPlace(i);
                        // GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        if(SceneView.sceneViews.Count > 0){
                            SceneView.FocusWindowIfItsOpen(typeof(SceneView));
                        }
                    }
                }

                bool oldEnable = true;

                if(i == 0){
                    oldEnable = GUI.enabled;
                    GUI.enabled = false;
                }

                if(GUILayout.Button(upTexture, GUILayout.Width(20f))){
                    GameObject actualObject = references.keys[i];
                    references.keys[i] = references.keys[i - 1];
                    references.keys[i - 1] = actualObject;
                }

                if(i == 0){
                    GUI.enabled = oldEnable;
                }

                if(i == (references.keys.Length - 1)){
                    oldEnable = GUI.enabled;
                    GUI.enabled = false;
                }

                if(GUILayout.Button(downTexture, GUILayout.Width(20f))){
                    GameObject actualObject = references.keys[i];
                    references.keys[i] = references.keys[i + 1];
                    references.keys[i + 1] = actualObject;
                }

                if(i == (references.keys.Length - 1)){
                    GUI.enabled = oldEnable;
                }
                
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void LoadScriptable(){
        if(references == null){
            return;
        }

        GameObject[] keys = new GameObject[references.ldBlocks.Count];
        List<GameObject> possibleKeys = new List<GameObject>(references.ldBlocks.Keys);

        int index = 0;

        if(references.keys != null){
            for(int i = 0; i < references.keys.Length; i++){
                if(possibleKeys.Contains(references.keys[i])){
                    keys[index] = references.keys[i];
                    possibleKeys.Remove(keys[index]);
                    index++;
                }
            }
        }

        while(possibleKeys.Count != 0){
            keys[index] = possibleKeys[possibleKeys.Count - 1];
            possibleKeys.RemoveAt(possibleKeys.Count - 1);
        }

        references.keys = keys;
        Selection.activeGameObject = null;
    }

    private Transform CreateLDElement(GameObject prefab, Transform parent){
        Transform result = (PrefabUtility.InstantiatePrefab(prefab) as GameObject).GetComponent<Transform>();
        // Vector3 localPos = result.localPosition;
        // result.parent = parent;
        // result.localPosition = localPos;
        result.parent = parent.GetChild(0);
        Quaternion actualRotation = result.parent.localRotation;
        result.parent.localRotation = Quaternion.identity;
        result.position = /* parent.GetChild(0).localRotation * localPos + */parent.position;
        result.localRotation = Quaternion.identity;

        result.parent.localRotation = actualRotation;

        return result;
    }

    private void ChangeObjectToPlace(GameObject obj){
        if(inPlacingObject != null){
            DestroyImmediate(inPlacingObject.gameObject);
        }

        objectToPlace = obj;
        inPlacingObject = (PrefabUtility.InstantiatePrefab(objectToPlace) as GameObject).GetComponent<Transform>();
        int flags = (int) HideFlags.None;
        // flags = Bitwise.Toggle(flags, (int) HideFlags.DontSaveInEditor);
        flags = Bitwise.Toggle(flags, (int) HideFlags.DontSaveInBuild);
        
        inPlacingObject.gameObject.hideFlags = (HideFlags) flags;
        // inPlacingObject.gameObject.hideFlags = HideFlags.DontSaveInEditor;

        SetRotation(inPlacingObject, lastProperties.rotation);

        if(lastProperties.hasCeiling){
            if(references.ldBlocks[objectToPlace].ceilingPrefab != null){
                CreateLDElement(references.ldBlocks[objectToPlace].ceilingPrefab, inPlacingObject);
            }
        }

        if(lastProperties.hasFloor){
            if(references.ldBlocks[objectToPlace].floorPrefab != null){
                CreateLDElement(references.ldBlocks[objectToPlace].floorPrefab, inPlacingObject);
            }
        }

        Selection.activeGameObject = inPlacingObject.gameObject;
        needRepaint = true;
    }

    private void ChangeObjectToPlace(int i){
        if(references.keys.Length <= i){
            return;
        }

        if(objectToPlace == references.keys[i]){
            return;
        }
        
        ChangeObjectToPlace(references.keys[i]);
    }

    private void RemoveObjectToPlace(){
        if(inPlacingObject != null){
            DestroyImmediate(inPlacingObject.gameObject);
        }

        objectToPlace = null;
        Selection.activeGameObject = null;
        actualSelection = null;
        selectedBloc = SelectedBloc.Empty;
        correctSelection = null;
    }

    private float GetNextPoint(float point, float step, float offset){
        return Mathf.Round((point - offset) / step) * step + offset;
    }

    private void ToggleRoof(){
        if(selectedBloc.roof != null){
            DestroyImmediate(selectedBloc.roof);
            selectedBloc.roof = null;
            lastProperties.hasCeiling = false;
        }
        else{
            Transform roof = CreateLDElement(references.ldBlocks[selectedBloc.reference].ceilingPrefab, selectedBloc.transform);
            selectedBloc.roof = roof.gameObject;
            lastProperties.hasCeiling = true;
        }
    }

    private void ToggleCeil(){
        if(selectedBloc.ceil != null){
            DestroyImmediate(selectedBloc.ceil);
            selectedBloc.ceil = null;
            lastProperties.hasFloor = false;
        }
        else{
            Transform ceil = CreateLDElement(references.ldBlocks[selectedBloc.reference].floorPrefab, selectedBloc.transform);
            selectedBloc.ceil = ceil.gameObject;
            lastProperties.hasFloor = true;
        }
    }

    private void RotateObject(Transform obj){
        Transform t = obj.GetComponent<Transform>().GetChild(0);
        float rotation = t.rotation.eulerAngles.y + 90f;
        t.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
        lastProperties.rotation = rotation;
    }

    private void SetRotation(Transform transform, float rotation){
        Transform t = transform.GetChild(0);
        t.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
    }

    private void MoveInPlacingObject(SceneView sceneView, Event guiEvent){
        if((guiEvent.mousePosition.x > 0f) && (guiEvent.mousePosition.x < sceneView.camera.scaledPixelWidth)){
            if((guiEvent.mousePosition.y > 0f) && (guiEvent.mousePosition.y < sceneView.camera.scaledPixelHeight)){
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                RaycastHit hit;
                GameObject rayGameObject;

                if(selectedBloc.transform != null){
                    selectedBloc.transform.GetChild(0).gameObject.SetActive(false);
                    rayGameObject = MeshRaycast.PickGameObject(guiEvent.mousePosition, true);
                    selectedBloc.transform.GetChild(0).gameObject.SetActive(true);
                }
                else{
                    rayGameObject = MeshRaycast.PickGameObject(guiEvent.mousePosition, true);
                }

                MeshFilter filter = rayGameObject?.GetComponent<MeshFilter>();

                if(filter != null){
                    if(MeshRaycast.IntersectRayMesh(mouseRay, filter, out hit)){
                        Vector3 point = hit.point;
                        point.x = GetNextPoint(point.x, references.step.x, references.offset.x);
                        point.y = GetNextPoint(point.y, references.step.y, references.offset.y);
                        point.z = GetNextPoint(point.z, references.step.z, references.offset.z);

                        inPlacingObject.position = point;
                    }
                }
            }
        }
    }
}
