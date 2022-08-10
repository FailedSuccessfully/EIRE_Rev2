using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInSequence : StateMachineBehaviour
{
    public int count = 1;
    private int counter;
    public SpellData spell;
    public float rate;
    public Vector2 offset;
    float time, freq;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        counter = count;
        time = 0;
        freq = 1 / rate;

    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        if (counter > 0 && time > freq)
        {
            counter--;
            time -= freq;
            SpellDriver sp = BattleManager.RequestSpell(spell, animator.GetInteger("PlayerIndex"));
            sp.transform.localPosition += (Vector3)offset;

        }
    }
}
