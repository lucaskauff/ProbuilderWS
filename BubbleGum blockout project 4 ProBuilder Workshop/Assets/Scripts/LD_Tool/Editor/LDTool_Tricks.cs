using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LDTool_Tricks : MonoBehaviour
{
    [MenuItem("Bubble Gum/LDTool Trick %g")]
    public static void LDTricks(){
        if(Selection.activeGameObject == null){
            return;
        }

        if(Selection.activeGameObject.name == "Visual"){
            Transform t = Selection.activeGameObject.transform;
            t.localPosition = new Vector3(3f, 0f, -3f);
            DestroyImmediate(t.GetChild(0).gameObject);

            Material mat = AssetDatabase.LoadAssetAtPath("Assets/Art/Materials/ArtScene/M_BrushStrokesTriplanar_Base.mat", typeof(Material)) as Material;

            EditorGUIUtility.systemCopyBuffer = t.parent.name;

            GameObject newObject = new GameObject("Mesh");
            Transform child = newObject.transform;
            child.parent = t;
            child.localPosition = new Vector3(-3f, 0f, 3f);
            child.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));

            newObject.AddComponent<MeshFilter>();
            newObject.AddComponent<MeshRenderer>().sharedMaterial = mat;
            newObject.AddComponent<MeshCollider>();
        }
    }
}
