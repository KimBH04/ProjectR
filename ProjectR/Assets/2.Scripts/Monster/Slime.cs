using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private GameObject splitSlime;
    
    protected override IEnumerator AttackPlayer()
    {
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
    }

    protected override void DieEnemy()
    {
        base.DieEnemy();
        Instantiate(splitSlime, transform.position, Quaternion.identity);
        Instantiate(splitSlime, transform.position, Quaternion.identity);
    }
    
    
    
    
}
