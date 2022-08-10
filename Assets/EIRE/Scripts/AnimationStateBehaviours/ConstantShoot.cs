using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantShoot : StateMachineBehaviour
{
    public float rate;
    public SpellData spell;
    public Vector2 offset;
    float time, freq;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time = 0;
        freq = 1 / rate;

    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        if (time > freq)
        {
            time -= freq;
            SpellDriver sp = BattleManager.RequestSpell(spell, animator.GetInteger("PlayerIndex"));
            sp.transform.localPosition += (Vector3)offset;

        }
    }
}
