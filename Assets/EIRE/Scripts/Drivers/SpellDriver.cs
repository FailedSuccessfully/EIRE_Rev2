using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

public class SpellDriver : Driver<SpellData>
{
    public string State;
    private PatternState moveState;
    public GameObject activeObject;
    public Vector3 target;
    private Stack<Type> stack;

    public override Driver<SpellData> Mount(SpellData ctx)
    {
        activeObject = GameObject.Instantiate(ctx.prefab, transform);

        stack = new Stack<Type>();
        foreach (PatternStates stateName in ctx.stateStack)
        {
            stack.Push(PatternState.MoveStateDictionary[stateName]);
        }

        gameObject.SetActive(true);
        return base.Mount(ctx);
    }

    void Start()
    {
        NextState();
    }

    private void SwitchState(PatternState state)
    {
        moveState?.OnStateExit();
        state.SetContext(this);
        moveState = state;
        moveState?.OnStateEnter();
        State = moveState.GetType().ToString();
    }
    public void NextState()
    {
        SwitchState((PatternState)Activator.CreateInstance(stack.Pop()));
    }
    public void SetTarget(Transform transform) => target = transform.position;

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
        {PatternStates.SplineRide, typeof(RideSplineMoveState)},
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
        handler = () => AttackUtilities.Movement.MoveForward(context.activeObject.transform, context.MountContext.speed);
        context.OnUpdate += handler;
    }

    public override void OnStateExit()
    {
        context.OnUpdate -= handler;
    }
}

public class RideSplineMoveState : PatternState
{
    Vector3 scale;
    SplineComputer spline;
    public override void OnStateEnter()
    {
        spline = context.gameObject.AddComponent<SplineComputer>();
        spline.editorAlwaysDraw = true;
        spline.is2D = true;

        /* TODO: This is hardcoded
        process:
            first point - instantiation point (local zero)
            mid point - point on circle (needs defined radius)
            last point -point of intersection between radius circle and target to midpoint line
        */
        SplinePoint[] points = AttackUtilities.Splines.CalculatePoints(context.MountContext.splineData.launcherRadius, context.target);
        spline.SetPoints(points, SplineComputer.Space.Local);
        TriggerGroup tg = new TriggerGroup();
        spline.triggerGroups = new TriggerGroup[1] { tg };
        SplineTrigger trig = new SplineTrigger(SplineTrigger.Type.Forward);
        trig.onCross.AddListener(() => context.NextState());
        tg.triggers = new SplineTrigger[1] { trig };
        trig.position = 1;
        spline.type = Spline.Type.BSpline;

        if (context.MountContext.splineData.Scaling != Vector3.zero)
        {
            scale = context.activeObject.transform.localScale;
            context.activeObject.transform.localScale = context.MountContext.splineData.Scaling;
        }


        AttackUtilities.Movement.RideSpline(context, spline);
    }

    public override void OnStateExit()
    {
        SplineFollower sFollower = context.activeObject.GetComponent<SplineFollower>();
        AttackUtilities.Movement.DetachSpline(sFollower);
        context.activeObject.transform.localScale = scale;
        GameObject.Destroy(spline);
    }
}

public class RotateToTargetState : PatternState
{
    Quaternion targetRotation;
    Quaternion currentRotation => context.activeObject.transform.localRotation;
    public override void OnStateEnter()
    {
        //todo: make rotation gradual
        Vector3 direction = (context.target - context.activeObject.transform.position).normalized;
        Debug.Log(direction);
        targetRotation = Quaternion.FromToRotation(Vector3.right, direction);
        context.activeObject.transform.localRotation = targetRotation;
        //context.StartCoroutine(Rotate());
        context.NextState();

    }

    public override void OnStateExit()
    {
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            context.activeObject.transform.localRotation = Quaternion.RotateTowards(currentRotation, targetRotation, 15f);
            yield return null;
        }
    }
}
