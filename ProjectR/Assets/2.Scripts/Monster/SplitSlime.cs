using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitSlime : Enemy
{
    protected override IEnumerator AttackPlayer()
    {
        IsAttack = true;
        print("Slime Attack");
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.SlimeMove);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
        IsAttack = false;
    }
}
