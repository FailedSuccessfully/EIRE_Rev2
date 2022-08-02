using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "EIRE/Character", order = 1)]
public class CharacterInfo : ScriptableObject
{
    public float Health, HealthRegen, Mana, ManaRegen, Barrier, BarrierRegen, BaseSpeed, MaxSpeed;
    public float puppetScale;
    public Vector3 puppetOffset;
    public GameObject puppet;

    public SpellData spellA, spellB, spellC;
}
