using System.Collections;
using UnityEngine;

public class Warrior : Enemy
{
    protected override IEnumerator AttackPlayer()
    {
        meleeArea.enabled = true;
        IsChase = false;
        IsAttack = true;
        Animator.SetBool(Attack,true);
        yield return new WaitForSeconds(1.2f);
        meleeArea.enabled = false;
        if (IsTingling)
        {
            yield break;
        }
        IsAttack = false;
        IsChase = true;
        Animator.SetBool(Attack,false);
        Animator.SetBool(Chase,true);
    }
}
