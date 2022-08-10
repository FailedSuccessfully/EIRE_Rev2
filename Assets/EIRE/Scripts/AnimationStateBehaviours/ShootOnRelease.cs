using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOnRelease : StateMachineBehaviour
{
    public Vector2 offset;
    public SpellData spell;
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        SpellDriver sp = BattleManager.RequestSpell(spell, animator.GetInteger("PlayerIndex"));
        sp.transform.localPosition += (Vector3)offset;
    }
}
