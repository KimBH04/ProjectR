using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class WarriorPresenter : MonoBehaviour
{
    public Collider meleeArea;
    public Transform player;
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
    public bool isHeal;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");
    
    
    private void Awake()
    {
        player= GameObject.FindWithTag("Player").GetComponent<Transform>();
        if (!player)
        {
            print("플레이어 업성");
        }
        
        PlayerController playerController = player.GetComponent<PlayerController>();
        _model = new EnemyModel(data.maxHp +(playerController.Level* 2), data.damage, data.speed, data.targetRadius, data.targetRange);
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
        StartCoroutine(IsHeal());
        Invoke(nameof(ChaseStart), 2f);

    }

    private void Update()
    {
        
        // RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, _model.TargetRadius, transform.forward, _model.TargetRange, LayerMask.GetMask("Player"));
        //
        // Debug.DrawRay(transform.position, transform.forward * _model.TargetRange, Color.red);
        
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

    private IEnumerator IsHeal()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (isHeal)
            {
                _model.CurrentHp += 10;
                _view.UpdateHpBar(_model.CurrentHp,_model.MaxHp);
            }
        }
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
        //_animator.SetBool(Chase,false);
        meleeArea.enabled = true;
        isChase = false;
        isAttack = true;
        _animator.SetBool(Attack,true);
        yield return new WaitForSeconds(1.2f);
        meleeArea.enabled = false;
        isAttack = false;
        isChase = true;
        _animator.SetBool(Attack,false);
        _animator.SetBool(Chase,true);
        
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
        if (_model.CurrentHp <= 0 && !isDead) 
        {
            DieEnemy();
        }
        else
        {
            StartCoroutine(OnDamage());
            // foreach (SkinnedMeshRenderer mesh in _meshRenderers)
            // {
            //     mesh.material.DOColor(Color.red, 0.1f).SetDelay(0.1f).OnComplete(() =>
            //     {
            //         mesh.material.DOColor(_originalMeshRenderers[mesh], 0.1f);
            //
            //     });
            // }
            
            
        }
    }
    
    private IEnumerator OnDamage()
    {
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

    // public IEnumerator OnDamage()
    // {
    //     isHit = true;
    //     isChase = false;
    //     isAttack = false;
    //     _animator.SetBool(Hit,true);
    //     foreach (SkinnedMeshRenderer mesh in _meshRenderers)
    //     {
    //         mesh.material.color = Color.red;
    //     }
    //
    //     yield return new WaitForSeconds(0.1f);
    //     
    //     foreach (SkinnedMeshRenderer mesh in _meshRenderers)
    //     {
    //         mesh.material.color = _originalMeshRenderers[mesh];
    //     }
    //     
    //     yield return new WaitForSeconds(0.13f);
    //     isHit = false;
    //     isChase = true;
    // }
    
    public void DieEnemy()
    {
        StopAllCoroutines();
        StartCoroutine(OnDie());
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyDead);
        _animator.SetBool(Attack,false);
        _animator.SetBool(Chase,false);
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
        
        // dissolveMaterial.DOFloat(1, "_DissolveAmount", 2);
       
        yield break;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skill") && !isHit && !isDead)
        {
            TakeDamage(10f);
        }
    }
}
