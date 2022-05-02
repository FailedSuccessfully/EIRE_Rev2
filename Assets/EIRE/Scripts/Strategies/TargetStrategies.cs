using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetStrategies
{
    OnceOnSpawn
}

public interface ITargetStrategy
{
    void SetTargeting(AttackDriver driver, Transform target);
}

public static class TargetStrategy
{
    private static Dictionary<TargetStrategies, ITargetStrategy> dict =
    new Dictionary<TargetStrategies, ITargetStrategy>() { { TargetStrategies.OnceOnSpawn, new TargetOnce() } };
    public static Dictionary<TargetStrategies, ITargetStrategy> TargetTable => dict;
}

public class TargetOnce : ITargetStrategy
{
    public void SetTargeting(AttackDriver driver, Transform target)
    {
        driver.OnEnableHook += () =>
        {
            Vector3 direction = (target.transform.position - driver.transform.position).normalized;
            driver.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        };
    }
}

