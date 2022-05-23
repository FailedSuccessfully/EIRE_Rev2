using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spline Data", menuName = "EIRE/Data/Spline", order = 1)]
public class SplineData : ScriptableObject
{
    public float SpeedMultiplyer;
    public float launcherRadius;
    public Vector3 Scaling;
}
