using System.Collections;
using UnityEngine;

public class Shield : Enemy
{
    protected override IEnumerator AttackPlayer()
    {
        Animator.SetBool(Chase,false);
        meleeArea.enabled = true;
        IsChase = false;
        IsAttack = true;
        Animator.SetBool(Attack,true);
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.ShieldGoblinAttack);
        yield return new WaitForSeconds(1.4f);
        if (IsTingling)
        {
            yield break;
        }
        IsAttack = false;
        IsChase = true;
        meleeArea.enabled = false;
        Animator.SetBool(Attack,false);
        Animator.SetBool(Chase,true);
    }
}
