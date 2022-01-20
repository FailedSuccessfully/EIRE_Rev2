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
        CharacterData P1;//, P2;
        var m = Addressables.LoadAssetAsync<CharacterInfo>("CharacterData/DefaultStats").WaitForCompletion();
        P1 = new CharacterData()
        {
            playerIndex = 0,
            Health = new Resource() { Max = m.Health, Current = m.Health, Rate = m.HealthRegen, Regen = isRegen(m.HealthRegen) },
            Mana = new Resource() { Max = m.Mana, Current = m.Mana, Rate = m.ManaRegen, Regen = isRegen(m.ManaRegen) },
            Barrier = new Resource() { Max = m.Barrier, Current = m.Barrier, Rate = m.BarrierRegen, Regen = isRegen(m.BarrierRegen) },
            BaseSpeed = m.BaseSpeed,
            MaxSpeed = m.MaxSpeed,
            Direction = Vector3.zero,
            Speed = Vector3.zero,
            AttackProperties = new AttackProps[] { m.propsA,
                                                    m.propsB,
                                                    m.propsC }

        };
        GameManager.CreateData<CharacterData>(GameManager.Players[0], this);
        GameManager.SetData<CharacterData>(GameManager.Players[0], P1);
    }
    private bool isRegen(float rate) => rate > 0;
}
