using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MonsterPresenter : MonoBehaviour
{
    public enum EState
    {
        Idle,
        Attack,
        Chase,
        Hit,
        Die,
    }
    
    [HideInInspector]
    public EnemyModel _model;   // 슬픈 사연이 있는 EnemyModel 클래스
    
    [HideInInspector]
    public EnemyView _view;     // 슬픈 사연이 있는 EnemyView 클래스
    
    // 적의 현재 상태
    public EState state = EState.Idle;
    
    public EnemyData data;               // 적 데이터
    public GameObject[] expStone;        // 경험치 돌 배열
    public float tranceDist = 10.0f;    // 추적 사거리
    public float attackDist = 2.0f;     // 공격 사거리
    
    // 몬스터 상태 여부
    public bool isChase;
    public bool isAttack;
    public bool isHit;
    public bool isDead;

    // 컴포넌트 캐시 처리
    private Transform _monsterTr;
    private Transform _playerTr;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rb;
    
    // 스킨 메쉬 렌더러 배열
    private SkinnedMeshRenderer[] _meshRenderers;
    private readonly Dictionary<SkinnedMeshRenderer, Color> _originalMeshRenderers = new Dictionary<SkinnedMeshRenderer, Color>();
    public Material dissolveMaterial;
    
    // 애니메이션 해시값
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");

    private void Awake()
    {
        Init();
        
        _monsterTr = GetComponent<Transform>();
        _playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _anim = GetComponent<Animator>();
        
    }

    private void Init()
    {
        _model = new EnemyModel(data.maxHp * 2, data.damage, data.speed, data.targetRadius, data.targetRange);
        _agent.speed = _model.Speed;
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            _originalMeshRenderers[mesh] = mesh.material.color;
        }
    }

    private void Start()
    {
        StartCoroutine(CheckEnemyState());
        StartCoroutine(MonsterAction());
    }

    private void Update()
    {
        if (_agent.remainingDistance >= 2.0f)
        {
            Vector3 direction = _agent.desiredVelocity;
            if(direction.sqrMagnitude >= 0.1f * 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                _monsterTr.rotation = Quaternion.Slerp(_monsterTr.rotation, rot, Time.deltaTime * 10.0f);
            }
        }
    }

    // private void FixedUpdate()
    // {
    //     FreezeVelocity();
    // }
    //
    // private void FreezeVelocity()
    // {
    //     if (isChase)
    //     {
    //         _rb.velocity = Vector3.zero;
    //         _rb.angularVelocity = Vector3.zero;
    //     }
    // }

    private IEnumerator CheckEnemyState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (state == EState.Die)
            {
                yield break;
            }
            
            float distance = Vector3.Distance(_monsterTr.position, _playerTr.position);
            
            if (distance <= attackDist)
            {
                state = EState.Attack;
            }
            else if (distance <= tranceDist)
            {
                state = EState.Chase;
            }
            else
            {
                state = EState.Idle;
            }
        }
    }
    
    private IEnumerator MonsterAction()
    {
        while (!isDead)
        {
            switch (state)
            {
                case EState.Idle:
                    _agent.isStopped = true;
                    _anim.SetBool(Idle, true);
                    break;
                
                case EState.Chase:
                    _agent.SetDestination(_playerTr.position);
                    _agent.isStopped = false;
                    _anim.SetBool(Chase, true);
                    _anim.SetBool(Attack, false);
                    break;
                
                case EState.Attack:
                    _anim.SetBool(Attack, true);
                    break;
                
                case EState.Hit:
                    _anim.SetTrigger(Hit);
                    foreach (SkinnedMeshRenderer mesh in _meshRenderers)
                    {
                        mesh.material.color = Color.red;
                    }

                    yield return new WaitForSeconds(0.1f);
        
                    foreach (SkinnedMeshRenderer mesh in _meshRenderers)
                    {
                        mesh.material.color = _originalMeshRenderers[mesh];
                    }
                    break;
                
                case EState.Die:
                    OnDie();
                    isDead = true;
                    _rb.isKinematic = true;
                    _agent.enabled = false;
                    _agent.isStopped = true;
                    _anim.SetTrigger(Die);
                    int randomIndex = Random.Range(0, expStone.Length);
                    Instantiate(expStone[randomIndex], _monsterTr.position, Quaternion.identity);
                    Destroy(gameObject, 2f);
                    break;
            }
        }
        yield break;
    }
    

    private void OnDie()
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
    }

    public void TakeDamage(float damage)
    {
        _model.CurrentHp -= damage;
        if (_model.CurrentHp <= 0 && !isDead)
        {
            state = EState.Die;
        }
        else
        {
            state = EState.Hit;
        }
    }
    
}
