using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Minotaur : Enemy
{
    [SerializeField] private GameObject _Effect;
    private Transform _cameraTr;
    
    private float timeSinceLastAttack;
    
    private Vector3 _lookVec;

    private bool isLook;
    
    
    private static readonly int Rush = Animator.StringToHash("Rush");
    private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
    private static readonly int Scream = Animator.StringToHash("Scream");
    private static readonly int Attack2 = Animator.StringToHash("Attack2");
    private static readonly int Attack3 = Animator.StringToHash("Attack3");
    private static readonly int Attack4 = Animator.StringToHash("Attack4");
    
    protected override void Awake()
    {
        base.Awake();
        _cameraTr = Camera.main?.transform;
        _Effect.SetActive(false);
    }
    
    

    public override void Update()
    {
        base.Update();

        if (isLook)
        {
            Vector3 direction = _playerTr.position - transform.position;
            direction.y = 0;
            
            if(direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }

        if (!IsAttack)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        
        
        if (!IsAttack && timeSinceLastAttack > 2.0f)
        {
            StartCoroutine(AttackPattern());
        }
        
    }
    

    protected override IEnumerator AttackPlayer()
    {
        IsAttack = true;
        IsChase = false;
        int intAction = Random.Range(0, 4);
        
        print(intAction);
        switch (intAction)
        {
            case 0:
                StartCoroutine(HitPattern1());
                break;
            case 1:
                StartCoroutine(HitPattern2());
                break;
            case 2:
                StartCoroutine(HitPattern3());
                break;
            case 3:
                StartCoroutine(HitPattern4());
                break;
        }
        timeSinceLastAttack = 0;
        yield return null;
    }

    private IEnumerator HitPattern1()
    {
        Animator.SetBool(Attack,true);
       
        yield return new WaitForSeconds(2.5f);
       
        Animator.SetBool(Attack,false);
        IsAttack = false;
        IsChase = true;
        
    }
    
    private IEnumerator HitPattern2()
    {
        Animator.SetBool(Attack2,true);
        yield return new WaitForSeconds(2.2f);
        Animator.SetBool(Attack2,false);
        IsAttack = false;
        IsChase = true;
    }
    
    private IEnumerator HitPattern3()
    {
        Animator.SetBool(Attack3,true);
       
        yield return new WaitForSeconds(1.7f);
       
        Animator.SetBool(Attack3,false);
        IsAttack = false;
        IsChase = true;
    }
    private IEnumerator HitPattern4()
    {
        Animator.SetBool(Attack4,true);
       
        yield return new WaitForSeconds(4.7f);
       
        Animator.SetBool(Attack4,false);
        IsAttack = false;
        IsChase = true;
    }

    private IEnumerator AttackPattern()
    {
        IsAttack = true;
        
        
        int intAction = Random.Range(0, 2);
        
        print(intAction);
        switch (intAction)
        {
            case 0:
                StartCoroutine(SkillAttack1());
                IsChase = false;
                break;
            case 1:
                StartCoroutine(SkillAttack2());
                break;
        }
        
        yield return null;
    }

    private IEnumerator SkillAttack1()
    {
        isLook = true;
        Animator.SetTrigger(Scream);
       
        yield return new WaitForSeconds(2.1f);
       
        Animator.SetTrigger(JumpAttack);
       
        
        yield return new WaitForSeconds(1.8f);
        isLook = false;
        _Effect.SetActive(true);
        _cameraTr.transform.DOShakePosition(1, 1, 10, 90);
        
        yield return new WaitForSeconds(1f);
        _Effect.SetActive(false);
        yield return new WaitForSeconds(0.8f);
        IsAttack = false;
        IsChase = true;
        
        timeSinceLastAttack = 0;
        
    }
    
    
    private IEnumerator SkillAttack2()
    {
        Animator.SetBool(Rush,true);
        _agent.speed = 20f;
        yield return new WaitForSeconds(2f);
        Animator.SetBool(Rush,false);
        _agent.speed = 5f;
        IsAttack = false;
        timeSinceLastAttack = 0;
    }
    
    
}

