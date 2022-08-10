using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellDriver : Driver<SpellData>
{
    public string State;
    private PatternState moveState;
    public GameObject activeObject;
    public Vector3 target;
    private Stack<Type> stack;
    Rigidbody rigid;

    public override Driver<SpellData> Mount(SpellData ctx)
    {
        activeObject = GameObject.Instantiate(ctx.prefab, transform);
        activeObject.transform.localScale = ctx.Scale;
        stack = new Stack<Type>();
        foreach (PatternStates stateName in ctx.stateStack)
        {
            //Debug.Log(stateName);
            stack.Push(PatternState.MoveStateDictionary[stateName]);
        }

        gameObject.SetActive(true);
        return base.Mount(ctx);
    }
    protected override void Awake()
    {
        rigid = activeObject.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        //rigid.constraints = RigidbodyConstraints.FreezeRotation;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        base.Awake();
    }
    void Start()
    {
        NextState();
        GetComponentInChildren<HurtBox>().RecieveOn();

    }

    protected override void Update()
    {
        base.Update();
        if (!BattleManager.InsideStageBoundaries(this))
        {
            BattleManager.RequestRelease(this);
            DestroyImmediate(activeObject);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Col");
        if (other.TryGetComponent<CharacterDriver>(out CharacterDriver d))
        {
            Debug.Log("Impact");
        }
    }

    private void SwitchState(PatternState state)
    {
        moveState?.OnStateExit();
        state.SetContext(this);
        moveState = state;
        moveState?.OnStateEnter();
        State = state.GetType().ToString();
    }
    public void NextState()
    {
        if (stack.Count > 0)
            SwitchState((PatternState)Activator.CreateInstance(stack.Pop()));
    }
    public void SetTarget(Transform transform)
    {
        target = transform.position;
        GetComponentInChildren<HurtBox>().SetLayer(transform.gameObject.layer);
    }

    public void Impact(CharacterDriver d)
    {
        ResourceManager.LoseResource(PlayerResource.Health, d.MountContext, 5);
        BattleManager.RequestRelease(this);
    }

}
public enum PatternStates
{
    Forward,
    SplineRide,
    Target
}

public abstract class PatternState
{
    public static readonly Dictionary<PatternStates, Type> MoveStateDictionary = new Dictionary<PatternStates, Type>() {
        {PatternStates.Forward, typeof(ForwardMoveState)},
        {PatternStates.Target, typeof(RotateToTargetState)}
    };
    protected SpellDriver context;
    public void SetContext(SpellDriver contextIn) => context = contextIn;
    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    protected Hook handler;
}

public class ForwardMoveState : PatternState
{
    public override void OnStateEnter()
    {
        Vector3 dir = context.target - context.transform.position;

        handler = () => AttackUtilities.Movement.MoveForward(context.transform, context.MountContext.speed, dir);
        context.OnUpdate += handler;
    }

    public override void OnStateExit()
    {
        context.OnUpdate -= handler;
    }
}


public class RotateToTargetState : PatternState
{
    Quaternion targetRotation;
    Quaternion currentRotation => context.activeObject.transform.localRotation;
    public override void OnStateEnter()
    {
        targetRotation = Quaternion.LookRotation(Vector3.Normalize(context.target - context.transform.localPosition));
        context.StartCoroutine(Rotate());
        //context.NextState();


    }

    public override void OnStateExit()
    {
    }

    IEnumerator Rotate()
    {
        yield return new WaitForEndOfFrame();
        context.NextState();
        while (true)
        {
            context.activeObject.transform.localRotation = Quaternion.RotateTowards(currentRotation, targetRotation, 90f);
            yield return null;
        }
    }
}
