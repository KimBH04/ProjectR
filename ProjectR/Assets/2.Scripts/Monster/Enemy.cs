using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public enum EType
    {
        Archer,
        Warrior,
        Shield,
    }
    
    
    [Header("Enemy Stats")]
    public EType type;

    
    public float maxHp;
    public float currentHp;
    public float damage;
    public float exp;
    public float coin;
    

    [Space(10)] 
    [Header("Enemy Components")]
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Animator _anim;

    public GameObject arrow;
    public Transform bulletPos;
    public Transform target;
    public Collider attackCollider;
    public Image hpBar;
    public GameObject hudDamageText;
    public Transform damageTextPos;
    
    [Space(10)]
    
    [Header("Enemy Flags")]
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public bool isHit;
    
    [Header("Enemy Renderers")]
    public Material dissolveMaterial;
    public SkinnedMeshRenderer[] meshRenderers;
    private readonly Dictionary<SkinnedMeshRenderer,Color> _originalColors = new Dictionary<SkinnedMeshRenderer, Color>();
    
    // 애니메이션
    private static readonly int OnHit = Animator.StringToHash("onHit");
    private static readonly int Die = Animator.StringToHash("onDie");
    private static readonly int IsWalk = Animator.StringToHash("isWalk");
    private static readonly int OnAttack = Animator.StringToHash("onAttack");
    private static readonly int OnDraw = Animator.StringToHash("onDraw");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
    
        foreach (SkinnedMeshRenderer mesh in meshRenderers)
        {
            _originalColors[mesh] = mesh.material.color;
        }
    }
    
    private void Start()
    {
        Invoke(nameof(ChaseStart),2f); // 1초 뒤 추적 시작
    }

    private void Update()
    {
        
        if(_agent.enabled)
        {
            _agent.SetDestination(target.position);
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
        _anim.SetBool(IsWalk,true);
        isChase = true;
    }

    private void ClosePlayer() // 맵이 좁기 때문에 쓰지 않을 듯
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer < 10f )
        {
            isChase = true;
            // 애니
        }
        else
        {
            isChase = false;
            // 애니
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
    
    private void Targeting()
    {
        if (!isDead)
        {
            float targetRadius = 0f;
            float targetRange = 0f;

            switch (type)
            {
                case EType.Warrior:
                    targetRadius = 1f;
                    targetRange = 1f;
                    break;
                
                case EType.Shield:
                    targetRadius = 1f;
                    targetRange = 2f;
                    break;
                
                case EType.Archer:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StopCoroutine(nameof(Attack));
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        yield return null;
        isChase = false;
        isAttack = true;
        // 애니메이션

        switch (type)
        {
            case EType.Warrior:
               _anim.SetTrigger(OnAttack);
                yield return new WaitForSeconds(1.1f);
                break;
            
            case EType.Shield:
                break;
            
            case EType.Archer:
                _anim.SetTrigger(OnDraw);
                yield return new WaitForSeconds(0.5f);
                _anim.SetTrigger(OnAttack);
                yield return new WaitForSeconds(0.3f);
                GameObject instantArrow = Instantiate(arrow, bulletPos.position, bulletPos.rotation);
                Rigidbody arrowRb = instantArrow.GetComponent<Rigidbody>();
                arrowRb.velocity = transform.forward * 20f;
                yield return new WaitForSeconds(1.4f);
                break;
        }

        
        isChase = true;
        isAttack = false;
        
    }

    

    

    private void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            
        }
        
        if(maxHp<currentHp)
        {
            currentHp = maxHp;
        }
        
        hpBar.fillAmount = currentHp / maxHp;
        GameObject hudText = Instantiate(hudDamageText);
        hudText.transform.position = hudText.transform.position; // 맞나? 체크 필요
        // 데미지 데이터 필요

        StartCoroutine(nameof(OnDamage));
    }

    private IEnumerator OnDamage()
    {
        
        if (currentHp > 0)
        {
            _anim.SetTrigger(OnHit);
            isHit = true;

            foreach (SkinnedMeshRenderer mesh in meshRenderers)
            {
                mesh.material.DOColor(Color.red, 0.1f).OnComplete(() =>
                {
                    mesh.material.DOColor(_originalColors[mesh], 0.1f);
                });
            }

            yield return new WaitForSeconds(0.1f);
            isHit = false;
        }
        else
        {
            OnDie();
        }

        yield return null;
    }

    private void OnDie()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(Dissolve));
        _anim.SetTrigger(Die);
        isDead = true;
        _agent.enabled = false;
        var o = gameObject;
        o.layer = 0;
        _rb.isKinematic= true;
        isChase = false;
        Destroy(o,1f);
    }

    private IEnumerator Dissolve()
    {
        foreach (SkinnedMeshRenderer mesh in meshRenderers)
        {
            Material[] materials = mesh.materials;
            
            for(int index =0; index < materials.Length; index++)
            {
                materials[index] = dissolveMaterial;
            }
            mesh.materials = materials;
        }

        dissolveMaterial.DOFloat(1, "_DissolveAmount",2);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Skill") && !isHit)
        {
            
            // 피격
            Debug.Log("피격");
            TakeDamage(10f);
        }
    }
}
