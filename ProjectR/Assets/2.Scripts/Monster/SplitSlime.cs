using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitSlime : Enemy
{
    protected override IEnumerator AttackPlayer()
    {
        IsAttack = true;
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.SlimeMove);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
        IsAttack = false;
    }
    
    protected override void DieEnemy()
    {
        StopAllCoroutines();
        StartCoroutine(OnDie());
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyDead);
        IsChase = false;
        IsAttack = false;
        _isDead = true;
        _agent.enabled = false;
        _rb.isKinematic = true;
        Invoke(nameof(gameObjectSetActive),2f);
        int randomIndex = Random.Range(0, expStone.Length);
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 2;
        Instantiate(expStone[randomIndex], spawnPosition, Quaternion.identity);
        
    }
    
    private void gameObjectSetActive()
    {
        gameObject.SetActive(false);
    }
}
