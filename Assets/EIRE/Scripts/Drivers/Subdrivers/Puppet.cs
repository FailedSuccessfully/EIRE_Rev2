using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : SubDriver
{
    CharacterDriver mainDriver;
    public Player MountContext => mainDriver.MountContext;
    GameObject puppet;
    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
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
        var d = GameManager.GetPlayerData<CharacterData>(MountContext);
        puppet = GameObject.Instantiate(d.Puppet, Vector3.zero, Quaternion.identity);
        puppet.transform.parent = transform;
    }

    public void FlipX(bool isFlip)
    {
        puppet.transform.localRotation = Quaternion.AngleAxis(isFlip ? 180 : 0, Vector3.up);
    }
}
