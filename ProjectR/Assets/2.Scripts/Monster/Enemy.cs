using System;
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
    
    public enum EState
    {
        Idle,
        Chase,
        Attack,
        Hit,
        Dead,
    }
    
    [Header("Enemy Stats")]
    public EType type;

    public float maxHp;
    public float currentHp;

    [Space(10)] [Header("Enemy Components")]
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator anim;
    
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
    private Dictionary<SkinnedMeshRenderer,Color> originalColors = new Dictionary<SkinnedMeshRenderer, Color>();

    private void Awake()
    {
        Init();
    }
    
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void Init()
    {
        rb.GetComponent<Rigidbody>();
        agent.GetComponent<NavMeshAgent>();
        anim.GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);

        foreach (SkinnedMeshRenderer mesh in meshRenderers)
        {
            originalColors[mesh] = mesh.material.color;
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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
                    targetRange = 2f;
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
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        // 애니메이션

        switch (type)
        {
            case EType.Warrior:
                yield return null;
                break;
            
            case EType.Shield:
                yield return null;
                break;
            
            case EType.Archer:
                yield return null;
                break;
        }

        isChase = true;
        isAttack = false;
        // 애니메이션
    }

    private IEnumerator OnDamage()
    {
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
