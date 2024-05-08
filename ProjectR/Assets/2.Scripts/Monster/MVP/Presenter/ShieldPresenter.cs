using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class ShieldPresenter : MonoBehaviour, IEnemyPresenter
{
    public Transform player;
    public Transform archer;
    public EnemyData data;
    public GameObject[] expStone;
    
    [HideInInspector]
    public EnemyModel _model;
    
    [HideInInspector]
    public EnemyView _view;
    
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
        GameObject playerPos = GameObject.FindWithTag("Player");
        if (playerPos != null)
        {
            player = playerPos.transform;
        }
        else
        {
            print("플레이어 업성");
        }

        _model = new EnemyModel(data.maxHp * 2, data.damage, data.speed, data.targetRadius, data.targetRange);
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _view = GetComponent<EnemyView>();
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
        _agent.speed = _model.Speed;
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            _originalMeshRenderers[mesh] = mesh.material.color;
        }
    }

    private void Start()
    {
        GameObject archerPos = GameObject.FindWithTag("Archer");
        if (archerPos != null)
        {
            archer = archerPos.transform;
        }
        else
        {
            print("아처 없쩌염 뿌우");
        }
        
        Invoke(nameof(ChaseStart), 2f);

    }

    private void Update()
    {
        if (archer && _agent.enabled)
        {
            Vector3 directionToArcher = archer.position - transform.position;
            directionToArcher.y = 0; 
            
            float distanceToArcher = directionToArcher.magnitude;
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            
            if (distanceToArcher > 1.5f)
            {
                
                transform.rotation = Quaternion.LookRotation(direction);
                _agent.SetDestination(archer.position);
                _agent.isStopped = !isChase;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(direction);
                _agent.isStopped = true;
            }
            
        }
        else if (_agent.enabled)
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
    
    public void Targeting()
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

    public IEnumerator AttackPlayer()
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
        if(_model.CurrentHp <= 0 && !isDead)
        {
            DieEnemy();
        }
        else
        {
            StartCoroutine(OnDamage());
        }
       
    }

    public IEnumerator OnDamage()
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
    
    public void DieEnemy()
    {
        StopAllCoroutines();
        StartCoroutine(OnDie());
        _animator.SetTrigger(Die);
        isChase = false;
        isAttack = false;
        isDead = true;
        _agent.enabled = false;
        _rb.isKinematic = true;
        Destroy(gameObject,2f);
        int randomIndex = Random.Range(0, expStone.Length);
        Instantiate(expStone[randomIndex], transform.position, Quaternion.identity);
    }

    public IEnumerator OnDie()
    {
        foreach (SkinnedMeshRenderer meshRenderer in _meshRenderers)
        {
            Material[] materials = meshRenderer.materials;
        
            for(int index = 0; index < materials.Length; index++)
            {
                materials[index] = dissolveMaterial;
            }
        
            meshRenderer.materials = materials;
            
            foreach (Material material in meshRenderer.materials)
            {
                material.DOFloat(1, "_DissolveAmount", 2);
            }
        }
        
        //dissolveMaterial.DOFloat(1, "_DissolveAmount", 2);
       
        yield break;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skill") && !isHit && !isDead) 
        {
            TakeDamage(10f);
        }
    }
}
