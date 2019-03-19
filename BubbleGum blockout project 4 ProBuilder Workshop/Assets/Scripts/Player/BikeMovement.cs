using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "BikeMovement", menuName = "BubbleGum/Bike Movement", order = 1)]
public class BikeMovement : ScriptableObject
{
    public float acceleration;
    public float maxSpeed;
    public bool canTurn = true;
    [ShowIf("canTurn")]
    public float turnSpeed;
}
