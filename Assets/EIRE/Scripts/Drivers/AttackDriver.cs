using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDriver : Driver<AttackProps>
{
    SpriteRenderer spr;
    public override Driver<AttackProps> Mount(AttackProps ctx)
    {

        return base.Mount(ctx);
    }
    void Awake()
    {
        spr = gameObject.AddComponent<SpriteRenderer>();

    }

    void OnEnable()
    {
        spr.enabled = false;
    }

    public void Show()
    {
        transform.localScale = Vector3.one * context.scale;
        Debug.Log(context.sprite);
        if (context.sprite)
            spr.sprite = context.sprite;
        spr.enabled = true;
    }
}
