using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDriver : MonoBehaviour
{
    public virtual void SetContext(GameObject parentDriver)
    {
        gameObject.layer = parentDriver.layer;
        transform.parent = parentDriver.transform;
    }
}

public class Shield : SubDriver{
  
    CharacterDriver mainDriver;
    public Player MountContext => mainDriver.MountContext;
    SphereCollider col;
    SpriteRenderer spr;
    Vector3 scale;
    void Awake()
    {
        col = gameObject.AddComponent<SphereCollider>();
        spr = gameObject.AddComponent<SpriteRenderer>();
        spr.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        spr.color = Color.blue * new Color(1, 1, 1, 0.3f);
        scale = Vector3.one;
    }
    void Start()
    {
        col.radius = 0.1f;
        gameObject.SetActive(false);
    }
    void FixedUpdate()
    {
        var d = GameManager.GetPlayerData<ResourceData>(mainDriver.MountContext);
        transform.localScale = scale * (d.RoundResources[2].Current / d.RoundResources[2].Max);
    }
    void Update()
    {
    }

    public override void SetContext(GameObject parentDriver)
    {
        if (!parentDriver.TryGetComponent<CharacterDriver>(out CharacterDriver cd)){
            Destroy(this);
        }
        mainDriver = cd;
        base.SetContext(parentDriver);
        scale = transform.parent.lossyScale * 10;
    }
}
