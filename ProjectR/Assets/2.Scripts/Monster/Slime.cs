using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private GameObject splitSlime;

    [HideInInspector] public SplitSlime[] mini = new SplitSlime[2];

    private new void Awake()
    {
        base.Awake();

        GameObject go1 = EnemyPools.AppearObject("Smallime");
        GameObject go2 = EnemyPools.AppearObject("Smallime");
        go1.SetActive(false);
        go2.SetActive(false);
        mini[0] = go1.GetComponent<SplitSlime>();
        mini[1] = go2.GetComponent<SplitSlime>();
        
    }
    
    
    

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

    protected override void DieEnemy()
    {
       base.DieEnemy();

        mini[0].transform.position = transform.position;
        mini[1].transform.position = transform.position;
        mini[0].gameObject.SetActive(true);
        mini[1].gameObject.SetActive(true);
    }
    
}
