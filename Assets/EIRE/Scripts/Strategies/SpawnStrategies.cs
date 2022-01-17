using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SpawnStrategies
{
    Immediate,
    Charge,
    Multiple
}
public interface ISpawnStrategy
{
    void SetSpawn(InputAction action, AttackProps props);
}

public static class SpawnStrategy
{
    private static Dictionary<SpawnStrategies, ISpawnStrategy> dict =
    new Dictionary<SpawnStrategies, ISpawnStrategy>() { { SpawnStrategies.Immediate, new SpawnImmediate() } };
    public static Dictionary<SpawnStrategies, ISpawnStrategy> SpawnTable => dict;

}

public class SpawnImmediate : ISpawnStrategy
{
    public void SetSpawn(InputAction action, AttackProps props)
    {
        action.started += ctx => { var a = BattleManager.RequestAttack(props); a.Show(); };
    }
}