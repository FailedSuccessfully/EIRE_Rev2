using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterManager : GameSystem
{
    public CharacterManager()
    {
        DataType = typeof(CharacterData);
    }

    public void InitPlayers()
    {
        CharacterData P1, P2;
        var stats = Addressables.LoadAssetAsync<CharacterInfo>("CharacterData/DefaultStats").WaitForCompletion();
        P1 = new CharacterData()
        {
            playerIndex = 0,
            Puppet = stats.puppet,
            Health = new Resource() { Max = stats.Health, Current = stats.Health, Rate = stats.HealthRegen, RegenLock = isRegen(stats.HealthRegen), TimerLock = true },
            Mana = new Resource() { Max = stats.Mana, Current = stats.Mana, Rate = stats.ManaRegen, RegenLock = isRegen(stats.ManaRegen), TimerLock = true },
            Barrier = new Resource() { Max = stats.Barrier, Current = stats.Barrier, Rate = stats.BarrierRegen, RegenLock = isRegen(stats.BarrierRegen), TimerLock = true },
            BaseSpeed = stats.BaseSpeed,
            MaxSpeed = stats.MaxSpeed,
            DashMult = 1,
            Direction = Vector3.zero,
            Speed = Vector3.zero,
            AttackProperties = new AttackProps[] { stats.propsA,
                                                    stats.propsB,
                                                    stats.propsC }

        };
        P2 = new CharacterData()
        {
            playerIndex = 1,
            Puppet = stats.puppet,
            Health = new Resource() { Max = stats.Health, Current = stats.Health, Rate = stats.HealthRegen, RegenLock = isRegen(stats.HealthRegen), TimerLock = true },
            Mana = new Resource() { Max = stats.Mana, Current = stats.Mana, Rate = stats.ManaRegen, RegenLock = isRegen(stats.ManaRegen), TimerLock = true },
            Barrier = new Resource() { Max = stats.Barrier, Current = stats.Barrier, Rate = stats.BarrierRegen, RegenLock = isRegen(stats.BarrierRegen), TimerLock = true },
            BaseSpeed = stats.BaseSpeed,
            MaxSpeed = stats.MaxSpeed,
            Direction = Vector3.zero,
            Speed = Vector3.zero,
            DashMult = 1,
            AttackProperties = new AttackProps[] { stats.propsA,
                                                    stats.propsB,
                                                    stats.propsC }

        };
        GameManager.CreateData<CharacterData>(GameManager.Players[0], this);
        GameManager.SetData<CharacterData>(GameManager.Players[0], P1);
        GameManager.CreateData<CharacterData>(GameManager.Players[1], this);
        GameManager.SetData<CharacterData>(GameManager.Players[1], P2);
    }
    private bool isRegen(float rate) => rate > 0;
}
