using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : SubDriver
{
    CharacterDriver mainDriver;
    public Player MountContext => mainDriver.MountContext;
    GameObject puppet;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale *= 3;
        if (MountContext.index == 0)
            mainDriver.FlipX();
    }

    public override void SetContext(GameObject parentDriver)
    {
        if (!parentDriver.TryGetComponent<CharacterDriver>(out CharacterDriver cd))
        {
            Destroy(this);
        }
        mainDriver = cd;

        base.SetContext(parentDriver);
        var d = GameManager.GetPlayerData<CharacterData>(MountContext);
        puppet = GameObject.Instantiate(d.Puppet, Vector3.zero, Quaternion.identity, transform);
    }
}
