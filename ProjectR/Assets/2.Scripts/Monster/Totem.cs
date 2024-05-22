using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Totem : MonoBehaviour
{
    public int maxHp;
    public int currentHp;

    public List<GameObject> enemies = new List<GameObject>();
    
    // public float healInterval;
    // public float healAmount;

    // private float _lastHealTime;
    private bool _isHit;

    public SkinnedMeshRenderer[] meshRenderers;
    public Material dissolveMaterial;
    private Dictionary <SkinnedMeshRenderer,Material> _originalColors = new Dictionary<SkinnedMeshRenderer, Material>();


    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
        
        foreach(SkinnedMeshRenderer mesh in meshRenderers)
        {
            _originalColors[mesh] = mesh.material;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.CompareTag("Warrior"))
            {
                Enemy warrior = enemy.GetComponent<Enemy>();
                warrior._isHeal = false;
            }
            
        }
    }
    // private void Start()
    // {
    //     _lastHealTime = Time.time;
    // }
    //
    // private void Update()
    // {
    //     if (Time.time - _lastHealTime > healInterval)
    //     {
    //         HealEnemies();
    //         _lastHealTime = Time.time;
    //     }
    //         
    // }
    //
    // private void HealEnemies()
    // {
    //     Collider[] enemies = Physics.OverlapSphere(transform.position, 15f,LayerMask.GetMask("Enemy"));
    //
    //     foreach (Collider collider in enemies)
    //     {
    //         if (collider.CompareTag("Warrior"))
    //         {
    //             WarriorPresenter enemy = collider.GetComponent<WarriorPresenter>();
    //             if (enemy != null)
    //             {
    //                 enemy._model.CurrentHp += 10;
    //                 enemy._view.UpdateHpBar(enemy._model.CurrentHp, enemy._model.MaxHp);
    //             }
    //         }
    //         else if (collider.CompareTag("Archer"))
    //         {
    //             ArcherPresenter enemy = collider.GetComponent<ArcherPresenter>();
    //             if (enemy != null)
    //             {
    //                 enemy._model.CurrentHp += 10;
    //                 enemy._view.UpdateHpBar(enemy._model.CurrentHp, enemy._model.MaxHp);
    //             }
    //         }
    //         else if (collider.CompareTag("Shield"))
    //         {
    //             ShieldPresenter enemy = collider.GetComponent<ShieldPresenter>();
    //             if (enemy != null)
    //             {
    //                 enemy._model.CurrentHp += 10;
    //                 enemy._view.UpdateHpBar(enemy._model.CurrentHp, enemy._model.MaxHp);
    //             }
    //         }
    //         else if (collider.CompareTag("Mino"))
    //         {
    //             MinoPresenter enemy = collider.GetComponent<MinoPresenter>();
    //             if (enemy != null)
    //             {
    //                 enemy._model.CurrentHp += 10;
    //                 enemy._view.UpdateHpBar(enemy._model.CurrentHp, enemy._model.MaxHp);
    //             }
    //         }
    //
    //
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skill")&&!_isHit)
        {
            StartCoroutine(OnHit());
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()._isHeal = true;
            enemies.Add(other.gameObject);
        }
        
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()._isHeal = false;
            enemies.Remove(other.gameObject);
        }
        
    }
    
    
    

    private IEnumerator OnHit()
    {
        _isHit = true;
        currentHp--;
        
        if (currentHp <= 0)
        {
            StartCoroutine(Dissolve());
            Destroy(gameObject, 1f);
        }
        else
        {
            foreach(SkinnedMeshRenderer mesh in meshRenderers)
            {
                mesh.material.color = Color.red;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            foreach(SkinnedMeshRenderer mesh in meshRenderers)
            {
                mesh.material = _originalColors[mesh];
            }
            _isHit = false;
        }
        
        yield return null;
    }
    
    private IEnumerator Dissolve()
    {
        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            Material[] materials = meshRenderer.materials;
        
            for(int index = 0; index < materials.Length; index++)
            {
                materials[index] = dissolveMaterial;
            }
        
            meshRenderer.materials = materials;
        }
        
        dissolveMaterial.DOFloat(1, "_DissolveAmount", 2);
        
        yield return null;
    }
}
