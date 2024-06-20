using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeathKnights : Enemy
{
    [SerializeField] private Transform attackCollision;


    private float timeSinceLastAttack;
    
    private Vector3 _lookVec;

    private bool isLook;
    private bool isStart;
    
    private static readonly int Rush = Animator.StringToHash("Rush");
    private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
    private static readonly int Attack2 = Animator.StringToHash("Attack2");
    private static readonly int Swing = Animator.StringToHash("Swing");
    private static readonly int start = Animator.StringToHash("Start");
    private static readonly int start2 = Animator.StringToHash("Start2");
    
    // protected override void Start()
    // {
    //     playerController = FindObjectOfType<PlayerController>();
    //
    //     StartCoroutine(IsHeal());
    //     if (!isBoss && _agent.enabled) 
    //     {
    //         // Animator.SetTrigger(start);
    //         // Animator.SetTrigger(start2);
    //         Invoke(nameof(ChaseStart), 1f);
    //     }
    //     
    //     
    // }


    public override void Update()
    {
        base.Update();

        if (_isDead)
        {
            return;
        }
        else
        {
            
            if (isLook)
            {
                Vector3 direction = _playerTr.position - transform.position;
                direction.y = 0;
            
                if (direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                }
            }

           
            
            if(!IsAttack)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            
            
            if (!IsAttack && timeSinceLastAttack >2f)
            {
                StartCoroutine(AttackPattern());
            }
            
            
        }
    }
    
    protected override IEnumerator AttackPlayer()
    {
       IsAttack = true;
       IsChase = false;
       int intAction = Random.Range(0, 2);

       switch (intAction)
       {
           case 0:
               StartCoroutine(HitPattern1());
                break;
           case 1:
               StartCoroutine(HitPattern2());
               break;
       }
       timeSinceLastAttack = 0;
       yield return null;
    }

    private IEnumerator HitPattern1()
    {
        Animator.SetBool(Attack, true);
        yield return new WaitForSeconds(3f);
        Animator.SetBool(Attack, false);
        IsAttack = false;
        IsChase = true;
        
    }
    
    private IEnumerator HitPattern2()
    {
        Animator.SetBool(Attack2, true);
        yield return new WaitForSeconds(2.8f);
        Animator.SetBool(Attack2, false);
        IsAttack = false;
        IsChase = true;
    }

    private IEnumerator AttackPattern()
    {
        IsAttack = true;
        
        int intAction = Random.Range(0, 2);
        switch (intAction)
        {
            case 0:
                StartCoroutine(SkillAttack1());
                break;
            
            case 1:
                StartCoroutine(SkillAttack2());
                break;
        }
        
        yield return null;
    }

    private IEnumerator SkillAttack1()
    {
        Animator.SetBool(Rush, true);
        _agent.speed = 20f;
        yield return new WaitForSeconds(2f);
        Animator.SetBool(Rush, false);
        Animator.SetTrigger(JumpAttack);
        IsChase = false;
        _agent.speed = 15f;
        yield return new WaitForSeconds(2.5f);
        _agent.speed = 5f;
        timeSinceLastAttack = 0;
        IsAttack = false;
        IsChase = true;
    }
    
    private IEnumerator SkillAttack2()
    {
        IsChase = false;
        Animator.SetTrigger(Swing);
       
        yield return new WaitForSeconds(1.5f);
        IsAttack = false;
        IsChase = true;
        timeSinceLastAttack = 0;
       
    }

    protected override void DieEnemy()
    {
        StopAllCoroutines();
        Animator.SetTrigger(Die);
        IsChase = false;
        IsAttack = false;
        _isDead = true;
        _agent.enabled = false;
        _rb.isKinematic = true;
        Invoke(nameof(SetActive),3f);
    }
    
    private void SetActive()
    {
        gameObject.SetActive(false);
    }


    private void AttackCollision()
    {
        int layerMask = LayerMask.GetMask("Player");
        RaycastHit[] rayHits = Physics.SphereCastAll(attackCollision.position, 9, Vector3.up, 0f, layerMask);

        foreach (RaycastHit hit in rayHits)
        {
            if(hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerController>().Hp -= 1;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCollision.position, 9);
    }
}
