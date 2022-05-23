using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProps : ScriptableObject, IDriveable
{
    [SerializeField]
    public float damage, pushback, cost, ttl;
    [SerializeField, Range(0.1f, 10f)] public float scale = 1f;
    [SerializeReference] public GameObject Effect;
    public MoveStrategies moveStrat;
    public SpawnStrategies spawnStrat;
    public TargetStrategies targetStrat;
    [SerializeField] public float speed = 1f;


    public void AcceptDriver(GameObject driver)
    {

        if (driver.TryGetComponent<Driver<AttackProps>>(out Driver<AttackProps> driveComponent))
        {
            driveComponent.Mount(this);
        }
    }
}

