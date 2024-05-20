using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public  abstract  class Enemy : MonoBehaviour
{
    [SerializeField] protected Collider meleeArea;
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject[] expStone;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject damageText;
    [SerializeField] private Transform damageTextPos;
    
    
    private EnemyModel _model;
    private Transform _playerTr;
    private EnemyView _view;
    protected Animator Animator;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private SkinnedMeshRenderer[] _meshRenderers;
    private readonly Dictionary<SkinnedMeshRenderer, Color> _originalMeshRenderers = new Dictionary<SkinnedMeshRenderer, Color>();
    public Material dissolveMaterial;
    
    protected bool IsChase;
    protected bool IsAttack;
    private bool _isDead;
    private bool _isHeal;
    protected bool IsTingling;
    
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Chase = Animator.StringToHash("Chase");
    protected static readonly int Die = Animator.StringToHash("Die");
    
    [HideInInspector]
    public UnityEvent onDieEvent = new UnityEvent();

    private void Awake()
    {
        _playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if (!_playerTr)
        {
            print("플레이어 업성");
        }
        
        PlayerController playerController = _playerTr.GetComponent<PlayerController>();
        _model = new EnemyModel(data.maxHp +(playerController.Level* 2), data.damage, data.speed, data.targetRadius, data.targetRange);
        Animator = GetComponent<Animator>();
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
        if (_agent.enabled && !IsTingling) 
        {
            _agent.SetDestination(_playerTr.position);
            _agent.isStopped = !IsChase;
        }
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void ChaseStart()
    {
        IsChase = true;
        Animator.SetTrigger(Chase);
    }

    private void Targeting()
    {
        RaycastHit[] rayHits = new RaycastHit[10];
        int hitCount = Physics.SphereCastNonAlloc(transform.position, _model.TargetRadius, transform.forward, rayHits, 
            _model.TargetRange, LayerMask.GetMask("Player"));
        if (hitCount > 0 && !IsAttack && !IsTingling)
        {
            StartCoroutine(AttackPlayer());
        }
    }
    
    
    
    private IEnumerator IsHeal()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(1f);
            if (_isHeal)
            {
                _model.CurrentHp += 10;
                _view.UpdateHpBar(_model.CurrentHp,_model.MaxHp);
            }
        }
        if (_isDead)
        {
            yield break;
        }
    }
    
    public void SetSpeed(float speed)
    {
        _agent.speed += speed;
    }

    public void OriginSpeed(float speed)
    {
        _agent.speed -= speed;
    }
    
    private void FreezeVelocity()
    {
        if (IsChase)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
    
    private void OnDisable()
    {
        onDieEvent.Invoke();
    }

    private void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.rectTransform.localScale = new Vector3(currentHp / maxHp, 1, 1);
    }

    private void ShowDamageText(float damage)
    {
        GameObject text = Instantiate(damageText, damageTextPos);
        text.GetComponent<DamageText>().damage= damage;
    }

    private void TakeDamage(float damage)
    {
        _model.CurrentHp-= damage;
        if(_model.CurrentHp <= 0)
        {
            _model.CurrentHp = 0;
            DieEnemy();
        }
        if(_model.CurrentHp > _model.MaxHp)
        {
            _model.CurrentHp = _model.MaxHp;
        }
        UpdateHpBar(_model.CurrentHp,_model.MaxHp);
        ShowDamageText(damage);
        StartCoroutine(OnDamage());
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

    private void DieEnemy()
    {
        StopAllCoroutines();
        StartCoroutine(OnDie());
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyDead);
        Animator.SetBool(Attack,false);
        Animator.SetBool(Chase,false);
        Animator.SetTrigger(Die);
        IsChase = false;
        IsAttack = false;
        _isDead = true;
        _agent.enabled = false;
        _rb.isKinematic = true;
        Destroy(gameObject,2f);
        int randomIndex = Random.Range(0, expStone.Length);
        Instantiate(expStone[randomIndex], transform.position, Quaternion.identity);
    }
    
    private IEnumerator OnDie()
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
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skill") && !_isDead)
        {
            // 임시 코드
            TakeDamage(10f);
        }
    }

    protected abstract IEnumerator AttackPlayer();
}
