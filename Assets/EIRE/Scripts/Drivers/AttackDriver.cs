using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackDriver : Driver<AttackProps>
{
    SpriteRenderer spr;
    SphereCollider hitSphere;
    float timer;
    public bool isTTL => !(timer > 0f);
    public override Driver<AttackProps> Mount(AttackProps ctx)
    {
        timer = ctx.ttl;
        return base.Mount(ctx);
    }
    void Awake()
    {
        spr = gameObject.AddComponent<SpriteRenderer>();
        hitSphere = gameObject.AddComponent<SphereCollider>();
        hitSphere.radius = 1f / context.scale;

    }
    protected override void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

    }

    protected override void OnEnable()
    {
        spr.enabled = false;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        spr.enabled = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * context.scale;
        if (context.sprite)
            spr.sprite = context.sprite;
        spr.enabled = true;
    }
}
