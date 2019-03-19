using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "LD_Tool_References", menuName = "BubbleGum/Create LD Tool", order = 2)]
public class LDToolReferences : SerializedScriptableObject
{
    [System.Serializable]
    public struct LDBloc{
        public GameObject ceilingPrefab;
        public GameObject floorPrefab;
        public GameObject[] variants;
    }

    [OdinSerialize] public Dictionary<GameObject, LDBloc> ldBlocks;

    [SerializeField, HideInInspector] public GameObject[] keys;
    [SerializeField, HideInInspector] public Vector3 step = Vector3.one;
    [SerializeField, HideInInspector] public Vector3 offset = Vector3.zero;


}
