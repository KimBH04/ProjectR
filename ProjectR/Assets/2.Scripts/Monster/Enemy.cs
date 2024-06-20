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
    [SerializeField] protected EnemyData data;
    [SerializeField] protected GameObject[] expStone;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject damageText;
    [SerializeField] private Transform damageTextPos;
    
    
    protected EnemyModel _model;
    protected Transform _playerTr;
    protected Animator Animator;
    protected Rigidbody _rb;
    protected NavMeshAgent _agent;
    protected SkinnedMeshRenderer[] _meshRenderers;

    protected readonly Dictionary<SkinnedMeshRenderer, Color[]> _originalMeshRenderers = new Dictionary<SkinnedMeshRenderer, Color[]>();
    public readonly Dictionary<SkinnedMeshRenderer, Material[]> _originalMaterials = new Dictionary<SkinnedMeshRenderer, Material[]>();
    public Material dissolveMaterial;


    public bool isBoss;
    protected bool IsChase;
    protected bool IsAttack;
    protected bool _isDead;
    public bool _isHeal;
    protected bool IsTingling;
    
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Chase = Animator.StringToHash("Chase");
    protected static readonly int Die = Animator.StringToHash("Die");
    
    [HideInInspector]
    public UnityEvent onDieEvent = new UnityEvent();

    protected PlayerController playerController;

    protected virtual void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            print("플레이어 업성");
        }
        else
        {
            _playerTr = player.transform;
        }

        PlayerController playerController = _playerTr.GetComponent<PlayerController>();
        if (playerController != null)
        {
            _model = new EnemyModel(data.enemyName,data.maxHp + (playerController.Level * 2), data.damage, data.speed, data.targetRadius, data.targetRange);
        }
        else
        {
            _model = new EnemyModel (data.enemyName,data.maxHp, data.damage, data.speed, data.targetRadius, data.targetRange);
        }
       
        Animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
        _agent.speed = _model.Speed;
        _agent.enabled = false;
        
        
        foreach (var meshRenderer in _meshRenderers)
        {
            _originalMaterials[meshRenderer] = meshRenderer.materials;
        }
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            Material[] materials = mesh.materials;
            Color[] originalColors = new Color[materials.Length];
            
        
            for (int index = 0; index < materials.Length; index++)
            {
                originalColors[index] = materials[index].color;
                
            }
        
            _originalMeshRenderers[mesh] = originalColors;
        }
        
        
        
        
        
    }
    

    protected virtual void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        _agent.enabled = true;

        // StartCoroutine(IsHeal());
        if ( _agent.enabled)
        {
            Invoke(nameof(ChaseStart), 1f);
        }
    }

    public virtual void Update()
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

    private void OnEnable()
    {
        
        if ( _agent.enabled) 
        {
            Invoke(nameof(ChaseStart), 1f);
        }

        if (_isDead)
        {
            Init();
        }
        
        
    }
    
    

    protected void Init()
    {
            _isDead = false;
            _agent.enabled = true;
            IsChase = false;
            IsAttack = false;
            _isDead = false;
            _agent.enabled = true;
            _rb.isKinematic = false;
        
            _model.CurrentHp = _model.MaxHp;
            UpdateHpBar(_model.CurrentHp,_model.MaxHp);
            
           
            
            foreach (SkinnedMeshRenderer meshRenderer in _meshRenderers)
            {
                foreach (Material material in meshRenderer.materials)
                {
                    material.DOFloat(0, "_Float", 0);
                }

                foreach (var mesh in _meshRenderers)
                {
                    if (_originalMaterials.ContainsKey(mesh))
                    {
                        meshRenderer.materials = _originalMaterials[meshRenderer];
                    }
                }
            }
            ChaseStart();


    }
    
    private void OnDisable()
    {
        
        _agent.enabled = false;
        onDieEvent.Invoke();
        
    }

    protected void ChaseStart()
    {
        IsChase = true;
        Animator.SetTrigger(Chase);
    }

    private void Targeting()
    {
        if ( !_isDead)
        {
            RaycastHit[] rayHits = new RaycastHit[10];
            int hitCount = Physics.SphereCastNonAlloc(transform.position, _model.TargetRadius, transform.forward,
                rayHits,
                _model.TargetRange, LayerMask.GetMask("Player"));
            if (hitCount > 0 && !IsAttack && !IsTingling)
            {
                StartCoroutine(AttackPlayer());
            }
            else if (playerController != null && playerController.isDie)
            {
                Tingling();
            }
        }
    }
    
    public void Tingling()
    { 
        IsTingling = true;
        IsAttack = false;
        IsChase = false;
        
        // Animator.SetBool(Attack,false);
        // Animator.SetBool(Chase,false);
    }
    
    public void EndTingling()
    {
        IsTingling = false;
        IsChase = true;
        Animator.SetBool(Chase,true);
    }
    
    
    
    // protected IEnumerator IsHeal()
    // {
    //     while (!_isDead)
    //     {
    //         yield return new WaitForSeconds(1f);
    //         if (_isHeal)
    //         {
    //             _model.CurrentHp += 10;
    //         }
    //         
    //        
    //     }
    //     
    //     if (_isDead)
    //     {
    //         yield break;
    //     }
    //     
    // }
    
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
    
    

    private void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.rectTransform.localScale = new Vector3(currentHp / maxHp, 1, 1);
    }

    private void ShowDamageText(float damage)
    {
        GameObject text = Instantiate(damageText, damageTextPos);
        text.GetComponent<DamageText>().damage= damage;
    }

    public void TakeDamage(int damage, Vector3 tr)
    {
        if (_isDead)
        {
            return;
        }
        
        _model.CurrentHp-= damage;
        if(_model.CurrentHp <= 0)
        {
            _model.CurrentHp = 0;
        }
        if(_model.CurrentHp > _model.MaxHp)
        {
            _model.CurrentHp = _model.MaxHp;
        }
        UpdateHpBar(_model.CurrentHp,_model.MaxHp);

        if (!isBoss)
        {
            ShowDamageText(damage);
        }
        

        if (_model.CurrentHp <= 0)
        {
            DieEnemy();
        }
        else
        {
            if (!isBoss)
            {
                ApplyKnockback(tr);
            }
            
            StartCoroutine(OnDamage());
        }
        
    }

    private IEnumerator OnDamage()
    {
        
        
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyHit);
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            Material[] materials = mesh.materials;
            
            for(int index =0; index < materials.Length; index++)
            {
                materials[index].color = Color.red;
            }
            
            
        }
        
     
        
        yield return new WaitForSeconds(0.1f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            if (_originalMeshRenderers.ContainsKey(mesh))
            {
                Material[] materials = mesh.materials;
                Color[] originalColors = _originalMeshRenderers[mesh];
            
                for (int index = 0; index < materials.Length; index++)
                {
                    materials[index].color = originalColors[index]; 
                }
            }
        }
    }
    
    // foreach (SkinnedMeshRenderer meshRenderer in _meshRenderers)
    // {
    //     Material[] materials = meshRenderer.materials;
    //     
    //     for(int index = 0; index < materials.Length; index++)
    //     {
    //         materials[index] = dissolveMaterial;
    //     }
    //     
    //     meshRenderer.materials = materials;
    //         
    //     foreach (Material material in meshRenderer.materials)
    //     {
    //         material.DOFloat(1, "_Float", 2);
    //     }
    // }

    protected virtual void DieEnemy()
    {
        StopAllCoroutines();
        foreach (SkinnedMeshRenderer meshRenderer in _meshRenderers)
        {
            if (_originalMeshRenderers.ContainsKey(meshRenderer))
            {
                Material[] materials = meshRenderer.materials;
                Color[] originalColors = _originalMeshRenderers[meshRenderer];

                for (int index = 0; index < materials.Length; index++)
                {
                    materials[index].color = originalColors[index];
                }

                meshRenderer.materials = materials; 
            }
        }
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
        Invoke(nameof(gameObjectSetActive),2f);
        //gameObject.SetActive(false);
        //Destroy(gameObject,2f);
        int randomIndex = Random.Range(0, expStone.Length);
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 2;
        Instantiate(expStone[randomIndex], spawnPosition, Quaternion.identity);
    }
    
    
    private void gameObjectSetActive()
    {
        gameObject.SetActive(false);
        EnemyPools.ReleaseObject(_model.EnemyName, gameObject);
    }
    
    protected IEnumerator OnDie()
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
                material.DOFloat(1, "_Float", 2);
            }
        }
        yield break;
    }
    
    
    
    

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Skill") && !_isDead)
        // {
        //     Vector3 hitDirection = (transform.position - other.transform.position).normalized;
        //     Vector3 knockbackPosition = transform.position + new Vector3(hitDirection.x, 0, hitDirection.z) * 2f;
        //     transform.DOMove
        //     (new Vector3(
        //         knockbackPosition.x, 
        //         transform.position.y, 
        //         knockbackPosition.z), 0.5f).SetEase(Ease.OutQuad);
        //     // int randomDamage = Random.Range(8, 13);
        //     // TakeDamage(randomDamage);
        //     // ApplyKnockback(other.transform);
        //     
        // }
    }

    private void ApplyKnockback(Vector3 sourcePosition)
    {
        Vector3 hitDirection = (transform.position - sourcePosition).normalized;
        Vector3 knockbackPosition = transform.position + new Vector3(hitDirection.x,0,hitDirection.z) * 2f;
        transform.DOMove(new Vector3(knockbackPosition.x, transform.position.y, knockbackPosition.z), 0.5f).SetEase(Ease.OutQuad);
    }
    
    

    protected abstract IEnumerator AttackPlayer();
}
