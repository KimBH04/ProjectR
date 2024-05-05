using UnityEngine;
using UnityEngine.AI;

public class EnemyPresenter : MonoBehaviour , IPresenter
{
    public GameObject player;
    public EnemyData data;
    private EnemyModel _model;
    private EnemyView _view;
    private Animator _animator;
    private Rigidbody _rb;
    private NavMeshAgent _agent;

    public bool isChase;
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");

    public enum EState
    {
        Idle,
        Attack,
        Chase,
        Hit,
        Die
    }
    
    private void Awake()
    {
        _model = new EnemyModel(data.maxHp, data.damage, data.targetRadius, data.targetRange);
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _view = GetComponent<EnemyView>();
    }

    private void Start()
    {
        if (player != null)
        {
            player = GameObject.FindWithTag("Player");
        }
        else
        {
            Debug.LogError("플레이어를 못 찾았어요 ㅠ");
        }
        
    }

    private void Update()
    {
        if (_agent.enabled)
        {
            _agent.SetDestination(player.transform.position);
            _agent.isStopped = !isChase;
        }
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    
    private void Targeting()
    {
        RaycastHit[] rayHits = new RaycastHit[10];
        int hitCount = Physics.SphereCastNonAlloc(transform.position, _model.TargetRadius, transform.forward, rayHits, _model.TargetRange, LayerMask.GetMask("Player"));
        if (hitCount > 0 && !isAttack)
        {
            // 공격
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

    public void AttackPlayer()
    {
        PlayAnimation(EState.Attack);
    }

    public void TakeDamage(float damage)
    {
        PlayAnimation(EState.Hit);
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
        
    }
    
    public void DieEnemy()
    {
        Destroy(gameObject,2f);
    }

    public void PlayAnimation(EState state)
    {
        switch (state)
        {
            case EState.Idle:
                _animator.Play(Idle);
                break;
            case EState.Attack:
                _animator.Play(Attack);
                break;
            case EState.Chase:
                _animator.Play(Chase);
                break;
            case EState.Hit:
                _animator.Play(Hit);
                break;
            case EState.Die:
                _animator.Play(Die);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skill"))
        {
            TakeDamage(10f);
        }
    }
}
