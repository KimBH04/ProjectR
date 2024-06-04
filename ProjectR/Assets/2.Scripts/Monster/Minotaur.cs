using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Minotaur : Enemy
{
    public bool isLook;

    private Vector3 _lookVec;
    private Vector3 _tauntVec;
    
    private static readonly int Scream = Animator.StringToHash("Scream");
    private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
    

    private void Start()
    {
        _agent.isStopped = true;
        StartCoroutine(AttackPlayer());
    }
    

    private void Update()
    {
        if (_isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            // float h = Input.GetAxisRaw("Horizontal");
            // float v = Input.GetAxisRaw("Vertical");
            // _lookVec = new Vector3(h, 0, v) * 5f;
            // transform.LookAt(_playerTr.position + _lookVec);

            // Vector3 directionToPlayer = (_playerTr.position - transform.position).normalized;
            // if (directionToPlayer != Vector3.zero)
            // {
            //     Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            //     transform.rotation = targetRotation;
            // }
            
            Vector3 direction = _playerTr.position - transform.position;
            direction.y = 0;
            
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                //transform.rotation = targetRotation;
            }
        }
        else
        {
            _agent.SetDestination(_tauntVec);
        }
    }

    protected override IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        
        int intAction = Random.Range(0, 3);

        print(intAction);
        switch (intAction)
        {
            case 0:
                StartCoroutine(Attack1());
                break;
            case 1:
                StartCoroutine(Attack2());
                break;
            case 2:
                StartCoroutine(Attack3());
                break;
        }
    }

    private IEnumerator Attack1()
    {
       
        
        Animator.SetTrigger( Scream);
        
        
        yield return new WaitForSeconds(2.5f);
        Animator.SetTrigger(JumpAttack);
        _tauntVec = _playerTr.position + _lookVec;
        isLook = false;
        _agent.isStopped = false;
        
        yield return new WaitForSeconds(3f);
        
        isLook = true;
        _agent.isStopped = true;
        StartCoroutine(AttackPlayer());
    }
    
    private IEnumerator Attack2()
    {
        isLook = false;
        _agent.isStopped = false;
        _tauntVec = _playerTr.position + _lookVec;
        Animator.SetTrigger(Chase);
        yield return new WaitForSeconds(2f);
        isLook = true;
        _agent.isStopped = true;
        StartCoroutine(AttackPlayer());
    }
    
    private IEnumerator Attack3()
    {
        Animator.SetTrigger(Attack);
        yield return new WaitForSeconds(2f);
        StartCoroutine(AttackPlayer());
    }
}

