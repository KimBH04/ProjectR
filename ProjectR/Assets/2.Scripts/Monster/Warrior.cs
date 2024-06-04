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
        yield return new WaitForSeconds(0.4f);
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.WarriorGoblinAttack);
        yield return new WaitForSeconds(0.8f);
       
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
