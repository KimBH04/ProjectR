using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    [SerializeField] private float arrowSpeed;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform firePos;
    
    private int _Draw = Animator.StringToHash("Draw");
    private int _Charge = Animator.StringToHash("Charge");
    
    protected override IEnumerator AttackPlayer()
    {
        Animator.SetBool(Chase, false);
        IsChase = false;
        IsAttack = true;
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.ChargingArrow);
        Animator.SetBool(_Draw, true);
        yield return new WaitForSeconds(0.7f);
        Animator.SetBool(_Charge, true);
        Animator.SetBool(_Draw, false);
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.ShootArrow);
        Animator.SetBool(Attack, true);
        Animator.SetBool(_Charge, false);
        GameObject instanceArrow = Instantiate(arrow, firePos.position, firePos.rotation);
        Rigidbody arrowRb = instanceArrow.GetComponent<Rigidbody>();
        arrowRb.velocity = firePos.forward * arrowSpeed;
        yield return new WaitForSeconds(1.0f);
        IsAttack = false;
        IsChase = true;
        Animator.SetBool(Attack, false);
        Animator.SetBool(Chase, true);
    }
}
