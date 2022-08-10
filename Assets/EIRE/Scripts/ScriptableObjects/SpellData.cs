using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "EIRE/Spell", order = 1)]
public class SpellData : ScriptableObject, IDriveable
{
    public GameObject prefab;
    public SplineData splineData;
    public float speed;
    [SerializeField]
    private Vector3 scale = Vector3.one;
    public Vector3 Scale => scale;
    public List<PatternStates> stateStack;
    public void AcceptDriver(GameObject driver)
    {
        if (driver.TryGetComponent<Driver<SpellData>>(out Driver<SpellData> driveComponent))
        {
            driveComponent.Mount(this);
        }
    }
}
