using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMagician : Enemy
{
    [SerializeField] private GameObject fireBall;
    [SerializeField] private Transform firePos;
    
    
    protected override IEnumerator AttackPlayer()
    {
        Animator.SetBool(Attack,true);
        IsChase = false;
        IsAttack = true;
        yield return new WaitForSeconds(0.7f);
        GameObject instanceFireBall = Instantiate(fireBall, firePos.position, firePos.rotation);
        Rigidbody fireBallRb = instanceFireBall.GetComponent<Rigidbody>();
        fireBallRb.velocity = firePos.forward * 10f;
        yield return new WaitForSeconds(1.9f);
        IsChase = true;
        IsAttack = false;
        Animator.SetBool(Attack,false);
        Animator.SetBool(Chase,true);
    }
}
