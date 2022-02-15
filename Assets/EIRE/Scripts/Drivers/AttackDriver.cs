using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackDriver : Driver<AttackProps>
{
    SpriteRenderer spr;
    /// they wont all be sphere so this is hardcoded temporarily
    SphereCollider hitSphere;
    float timer;
    public bool isTTL => !(timer > 0f);
    public override Driver<AttackProps> Mount(AttackProps ctx)
    {
        timer = ctx.ttl;
        return base.Mount(ctx);
    }
    protected override void Awake()
    {
        spr = gameObject.AddComponent<SpriteRenderer>();
        hitSphere = gameObject.AddComponent<SphereCollider>();
        hitSphere.isTrigger = true;
        hitSphere.radius = 1f / context.scale;
        base.Awake();
    }
    protected override void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

    }

    protected override void OnEnable()
    {
        spr.enabled = false;
        hitSphere.enabled = false;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        spr.enabled = false;
        hitSphere.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.TryGetComponent<Shield>(out Shield shield))
        {
            ResourceManager.LoseResource(PlayerResource.Barrier, shield.MountContext, MountContext.damage * 0.75f);
            BattleManager.RequestRelease(this);
        }
        else if (other.gameObject.TryGetComponent<CharacterDriver>(out CharacterDriver driver))
        {
            ResourceManager.LoseResource(PlayerResource.Health, driver.MountContext, MountContext.damage);
            other.attachedRigidbody.AddForceAtPosition(transform.right * MountContext.pushback, (other.transform.position - transform.position), ForceMode.Impulse);

            BattleManager.RequestRelease(this);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * context.scale;
        if (context.sprite)
            spr.sprite = context.sprite;
        spr.enabled = true;
        hitSphere.enabled = true;
    }
}
