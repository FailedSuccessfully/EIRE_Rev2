using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : SubDriver
{
    static int count = 0;
    int num;
    CharacterDriver mainDriver;
    public Player MountContext => mainDriver.MountContext;
    GameObject puppet;
    void Awake()
    {
        num = count++;
        Debug.Log($"puppet awake {num}");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"puppet start {num}");
        mainDriver.FlipX(MountContext.index == 0 ? true : false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void SetContext(GameObject parentDriver)
    {
        if (!parentDriver.TryGetComponent<CharacterDriver>(out CharacterDriver cd))
        {
            Destroy(this);
        }
        mainDriver = cd;

        base.SetContext(parentDriver);
        Debug.Log($"Context set {num}");
        var d = GameManager.GetPlayerData<CharacterData>(MountContext);
        puppet = GameObject.Instantiate(d.Puppet, Vector3.zero, Quaternion.identity, transform);
        Debug.Log($"Puppet instantiate {mainDriver.name}");
    }
}
