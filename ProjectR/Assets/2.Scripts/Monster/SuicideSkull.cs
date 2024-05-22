using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideSkull : Enemy
{
    [SerializeField] 
    private GameObject explosionEffect;
    
    protected override IEnumerator AttackPlayer()
    {
        IsChase = false;
        IsAttack = true;
        yield return new WaitForSeconds(2f);
        explosionEffect.SetActive(true);
        RaycastHit[] rayHits =Physics.SphereCastAll(transform.position,15,Vector3.up,0f,LayerMask.GetMask("Player"));

        foreach (RaycastHit hitObj in rayHits)
        {
            // hitObj.transform.GetComponent<PlayerController>.TakeDamage(10f);
        }
        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
}
