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
    public List<PatternStates> stateStack;
    private Stack<Type> stack;
    public void AcceptDriver(GameObject driver)
    {
        stack = new Stack<Type>();
        foreach (PatternStates stateName in stateStack)
        {
            stack.Push(PatternState.MoveStateDictionary[stateName]);
        }

        if (driver.TryGetComponent<Driver<SpellData>>(out Driver<SpellData> driveComponent))
        {
            driveComponent.Mount(this);
        }
    }

    public Type PopState() => stack.Pop();
}
