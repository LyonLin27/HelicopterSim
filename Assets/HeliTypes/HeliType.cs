using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="HeliType")]
public class HeliType : ScriptableObject
{
    [Header ("Attributes")]
    public int HP = 100;

    [Header("Physics")]
    public ForceMode forceMode;
    public float mass = 120f;
    public float acc = 400f;
    public float dcc = 250f;
    public float res = 100f;
    public float tltAcc = 100f;
    public float tltMax = 60f;
    public float rotAcc = 100f;

    [Header("Physics Handicap")]
    public float lowForceThreshold = 1000;
    public float highForceThreshold = 4000;
    public float highForceDropRange = 1000;
    public float balanceTorque = 100f;
}
