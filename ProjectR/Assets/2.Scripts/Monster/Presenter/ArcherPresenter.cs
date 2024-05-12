using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class ArcherPresenter : MonoBehaviour
{
    public float arrowSpeed;
    public Transform player;
    public EnemyData data;
    public GameObject arrow;
    public Transform firePos;
    public GameObject[] expStone;
    
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
    public bool isClose;
    public bool isStart;
    public bool isHeal;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Draw = Animator.StringToHash("Draw");
    private static readonly int Charge = Animator.StringToHash("Charge");
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
        
       float distance = Vector3.Distance(transform.position, player.position);
      
       // RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, _model.TargetRadius, transform.forward, _model.TargetRange, LayerMask.GetMask("Player"));
       //
       // // DrawRay로 레이 시각화
       // Debug.DrawRay(transform.position, transform.forward * _model.TargetRange, Color.red);

       if (distance <= 5f && !isDead && !isHit && !isAttack && isStart)  
       {
           Vector3 direction = player.position - transform.position;
           direction.y = 0;

           Vector3 fleePosition = direction.normalized - direction.normalized * 4f;
            
           transform.rotation = Quaternion.LookRotation(direction);
            
           _agent.SetDestination(fleePosition);
           _agent.isStopped = false;
           isClose = true;
       }
       else if (_agent.enabled && isStart)
       {
           isClose = false;
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
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);
            if (isHeal)
            {
                _model.CurrentHp += 10;
                _view.UpdateHpBar(_model.CurrentHp,_model.MaxHp);
            }
        }
        if (isDead)
        {
            yield break;
        }
    }

    private void ChaseStart()
    {
        isStart = true;
        isChase = true;
        _animator.SetBool(Chase,true);
    }
    
    public void Targeting()
    {
        RaycastHit[] rayHits = new RaycastHit[1];
        int hitCount = Physics.SphereCastNonAlloc(transform.position, _model.TargetRadius, transform.forward, rayHits, 
            _model.TargetRange, LayerMask.GetMask("Player"));
        if (hitCount > 0 && !isAttack && !isHit && !isDead)
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
        if (!isClose && !isDead && !isHit && !isAttack)
        {
            _animator.SetBool(Chase, false);
            isChase = false;
            isAttack = true;
            AudioManager.Instance.PlaySfx(AudioManager.ESfx.ChargingArrow);
            _animator.SetBool(Draw, true);
            yield return new WaitForSeconds(0.7f);
            _animator.SetBool(Charge, true);
            _animator.SetBool(Draw, false);
            yield return new WaitForSeconds(1f);
            AudioManager.Instance.PlaySfx(AudioManager.ESfx.ShootArrow);
            _animator.SetBool(Attack, true);
            _animator.SetBool(Charge, false);
            GameObject instanceArrow = Instantiate(arrow, firePos.position, firePos.rotation);
            Rigidbody arrowRb = instanceArrow.GetComponent<Rigidbody>();
            arrowRb.velocity = firePos.forward * arrowSpeed;
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
            isChase = true;
            _animator.SetBool(Attack, false);
            _animator.SetBool(Chase, true);
        }
       

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
    //     StopCoroutine(AttackPlayer());
    //     _animator.SetBool(Chase,false);
    //     _animator.SetBool(Draw,false);
    //     _animator.SetBool(Charge,false);
    //     _animator.SetBool(Attack,false);
    //     
    //     AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyHit);
    //     isHit = true;
    //     isChase = false;
    //     isAttack = false;
    //     _animator.SetTrigger(Hit);
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
    //     isChase = true;
    //     isHit = false;
    //     _animator.SetBool(Chase,true);
    // }
    
    private void DieEnemy()
    {
        StopAllCoroutines();
        StartCoroutine(OnDie());
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.EnemyDead);
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
        if(other.CompareTag("Skill"))
        {
            TakeDamage(10f);
        }
    }
}
