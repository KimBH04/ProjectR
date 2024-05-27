using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Minotaur : MonoBehaviour
{
    [SerializeField] private float hpBar;
    [SerializeField] private GameObject damageText;
    [SerializeField] private Transform hpBarPrefab;
    
    
    private Transform _playerTr;
    private bool _isLook;
    private bool _isDead;
    
    private Vector3 _lookVec;
    private Vector3 _tauntVec;
    
    private Animator _animator;
    private NavMeshAgent _agent;
    private Rigidbody _rigid;


    private void Awake()
    {
        _playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rigid = GetComponent<Rigidbody>();

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

        if (_isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(_playerTr.position + _lookVec);
        }
        else
        {
            _agent.SetDestination(_tauntVec);
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }
    
    private void FreezeVelocity()
    {
        _rigid.angularVelocity = Vector3.zero;
        _rigid.velocity = Vector3.zero;
    }

    private  IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        
        int ranAction = Random.Range(0, 3);

        switch (ranAction)
        {
            case 0:
                StartCoroutine(Pattern1());
                break;
            case 1:
                StartCoroutine(Pattern2());
                break;
            case 2:
                StartCoroutine(Pattern3());
                break;
        }
        
    }

    private IEnumerator Pattern1()
    {
        yield return null;
        StartCoroutine(AttackPlayer());
    }
    
    private IEnumerator Pattern2()
    {
        yield return null;
        StartCoroutine(AttackPlayer());
    }
    
    private IEnumerator Pattern3()
    {
        yield return null;
        StartCoroutine(AttackPlayer());
    }
}

