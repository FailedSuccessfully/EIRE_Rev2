using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStrategies
{
    None,
    Forward,
    ToPosition
}
public interface IMoveStrategy
{
    Vector3 Move(Driver<AttackProps> attackDriver);
}

public static class MoveStrategy
{
    private static Dictionary<MoveStrategies, IMoveStrategy> dict =
    new Dictionary<MoveStrategies, IMoveStrategy>() { { MoveStrategies.Forward, new MoveForward() } };
    public static Dictionary<MoveStrategies, IMoveStrategy> MoveTable => dict;

}



public class MoveForward : IMoveStrategy
{
    public Vector3 Move(Driver<AttackProps> attackDriver)
    {
        return attackDriver.transform.position + attackDriver.transform.right * attackDriver.MountContext.speed * Constants.Speed.Coefficient;
    }
}