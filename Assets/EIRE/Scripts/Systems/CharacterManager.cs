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

    public override void InitPlayers()
    {
        //TODO: find a more concise way to write this
        CharacterData P1, P2;
        var stats1 = Addressables.LoadAssetAsync<CharacterInfo>("CharacterData/Felicia").WaitForCompletion();
        var stats2 = Addressables.LoadAssetAsync<CharacterInfo>("CharacterData/DefaultStats").WaitForCompletion();
        P1 = new CharacterData()
        {
            playerIndex = 0,
            Puppet = stats1.puppet,
            Health = new Resource() { Max = stats1.Health, Current = stats1.Health, Rate = stats1.HealthRegen, RegenLock = isRegen(stats1.HealthRegen), TimerLock = true },
            Mana = new Resource() { Max = stats1.Mana, Current = stats1.Mana, Rate = stats1.ManaRegen, RegenLock = isRegen(stats1.ManaRegen), TimerLock = true },
            Barrier = new Resource() { Max = stats1.Barrier, Current = stats1.Barrier, Rate = stats1.BarrierRegen, RegenLock = isRegen(stats1.BarrierRegen), TimerLock = true },
            BaseSpeed = stats1.BaseSpeed,
            MaxSpeed = stats1.MaxSpeed,
            DashMult = 1,
            Direction = Vector3.zero,
            Speed = Vector3.zero,
            AttackProperties = new AttackProps[] { stats1.propsA,
                                                    stats1.propsB,
                                                    stats1.propsC }

        };
        P2 = new CharacterData()
        {
            playerIndex = 1,
            Puppet = stats2.puppet,
            Health = new Resource() { Max = stats2.Health, Current = stats2.Health, Rate = stats2.HealthRegen, RegenLock = isRegen(stats2.HealthRegen), TimerLock = true },
            Mana = new Resource() { Max = stats2.Mana, Current = stats2.Mana, Rate = stats2.ManaRegen, RegenLock = isRegen(stats2.ManaRegen), TimerLock = true },
            Barrier = new Resource() { Max = stats2.Barrier, Current = stats2.Barrier, Rate = stats2.BarrierRegen, RegenLock = isRegen(stats2.BarrierRegen), TimerLock = true },
            BaseSpeed = stats2.BaseSpeed,
            MaxSpeed = stats2.MaxSpeed,
            Direction = Vector3.zero,
            Speed = Vector3.zero,
            DashMult = 1,
            AttackProperties = new AttackProps[] { stats2.propsA,
                                                    stats2.propsB,
                                                    stats2.propsC }

        };
        GameManager.CreateData<CharacterData>(GameManager.Players[0], this);
        GameManager.SetData<CharacterData>(GameManager.Players[0], P1);
        GameManager.CreateData<CharacterData>(GameManager.Players[1], this);
        GameManager.SetData<CharacterData>(GameManager.Players[1], P2);
    }
    private bool isRegen(float rate) => rate > 0;
}
