using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "EIRE/Character", order = 1)]
public class CharacterInfo : ScriptableObject
{
    public float Health, HealthRegen, Mana, ManaRegen, Barrier, BarrierRegen, BaseSpeed, MaxSpeed;
    public GameObject puppet;

    public AttackProps propsA, propsB, propsC;
    public SpellData spellA, spellB, spellC;
}
