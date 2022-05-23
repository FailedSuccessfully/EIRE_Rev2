using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpellData spellData;
    public GameObject Target;
    SpellDriver testDriver;
    // Start is called before the first frame update
    void Start()
    {
        testDriver = gameObject.AddComponent<SpellDriver>();
        testDriver.SetTarget(Target.transform);
        spellData.AcceptDriver(gameObject);

    }
    void Update()
    {
    }
}
