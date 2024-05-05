using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyPresenter : MonoBehaviour 
{
    public Transform player;
    public EnemyData data;
    private EnemyModel _model;
    private EnemyView _view;
    private Animator _animator;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private SkinnedMeshRenderer[] _meshRenderers;
    private readonly Dictionary<SkinnedMeshRenderer, Color> _originalMeshRenderers = new Dictionary<SkinnedMeshRenderer, Color>();
    public Material dissolveMaterial;
    
    public bool isChase;
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");
    
    
    private void Awake()
    {
        _model = new EnemyModel(data.maxHp, data.damage, data.speed, data.targetRadius, data.targetRange);
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _view = GetComponent<EnemyView>();
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            _originalMeshRenderers[mesh] = mesh.material.color;
        }
    }

    private void Start()
    {
        _agent.speed = _model.Speed;
        Invoke(nameof(ChaseStart), 2f);

    }

    private void Update()
    {
        if (_agent.enabled)
        {
            _agent.SetDestination(player.position);
            _agent.isStopped = !isChase;
        }
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void ChaseStart()
    {
        isChase = true;
        _animator.SetBool(Chase,true);
    }
    
    private void Targeting()
    {
        RaycastHit[] rayHits = new RaycastHit[10];
        int hitCount = Physics.SphereCastNonAlloc(transform.position, _model.TargetRadius, transform.forward, rayHits, 
            _model.TargetRange, LayerMask.GetMask("Player"));
        if (hitCount > 0 && !isAttack)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    IEnumerator AttackPlayer()
    {
        isChase = false;
        isAttack = true;
        _animator.SetTrigger(Attack);
        yield return new WaitForSeconds(3f);
        isAttack = false;
        isChase = true;
        
    }

    public void TakeDamage(float damage)
    {
        _model.CurrentHp -= damage;
        print(_model.CurrentHp);
        if (_model.CurrentHp <= 0)
        {
            _model.CurrentHp = 0;
        }
        if(_model.CurrentHp > _model.MaxHp)
        {
            _model.CurrentHp = _model.MaxHp;
        }
        _view.UpdateHpBar(_model.CurrentHp,_model.MaxHp);
        _view.ShowDamageText(damage);
        if(_model.CurrentHp <= 0)
        {
            DieEnemy();
        }
        else
        {
            StartCoroutine(OnDamage());
        }
       
    }

    IEnumerator OnDamage()
    {
        _animator.SetTrigger(Hit);
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }
    }
    
    private void DieEnemy()
    {
        StartCoroutine(OnDie());
        _animator.SetTrigger(Die);
        isChase = false;
        isAttack = false;
        isDead = true;
        _agent.enabled = false;
        _rb.isKinematic = true;
        Destroy(gameObject,1f);
    }

    IEnumerator OnDie()
    {
        foreach (SkinnedMeshRenderer meshRenderer in _meshRenderers)
        {
            Material[] materials = meshRenderer.materials;
        
            for(int index = 0; index < materials.Length; index++)
            {
                materials[index] = dissolveMaterial;
            }
        
            meshRenderer.materials = materials;
        }
        
        dissolveMaterial.DOFloat(1, "_DissolveAmount", 2);
       
        yield break;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skill"))
        {
            TakeDamage(10f);
        }
    }
}
