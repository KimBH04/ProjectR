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

    public MeshRenderer[] meshRenderers;
    public Material dissolveMaterial;
    private Dictionary <MeshRenderer,Material> _originalColors = new Dictionary<MeshRenderer, Material>();


    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        dissolveMaterial = new Material(dissolveMaterial);
        
        foreach(MeshRenderer mesh in meshRenderers)
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
                WarriorPresenter warrior = enemy.GetComponent<WarriorPresenter>();
                warrior.isHeal = false;
            }
            else if (enemy.CompareTag("Archer"))
            {
                ArcherPresenter archer = enemy.GetComponent<ArcherPresenter>();
                archer.isHeal = false;
            }
            else if (enemy.CompareTag("Shield"))
            {
                ShieldPresenter shield = enemy.GetComponent<ShieldPresenter>();
                shield.isHeal = false;
            }
            else if (enemy.CompareTag("Mino"))
            {
                MinoPresenter mino = enemy.GetComponent<MinoPresenter>();
                mino.isHeal = false;
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

        if (other.CompareTag("Warrior"))
        {
            other.GetComponent<WarriorPresenter>().isHeal = true;
            enemies.Add(other.gameObject);
        }
        else if (other.CompareTag("Archer"))
        {
            other.GetComponent<ArcherPresenter>().isHeal = true;
            enemies.Add(other.gameObject);
        }
        else if (other.CompareTag("Shield"))
        {
            other.GetComponent<ShieldPresenter>().isHeal = true;
            enemies.Add(other.gameObject);
        }
        else if (other.CompareTag("Mino"))
        {
            other.GetComponent<MinoPresenter>().isHeal = true;
            enemies.Add(other.gameObject);
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Warrior"))
        {
            other.GetComponent<WarriorPresenter>().isHeal = false;
            enemies.Remove(other.gameObject);
        }
        else if (other.CompareTag("Archer"))
        {
            other.GetComponent<ArcherPresenter>().isHeal = true;
            enemies.Remove(other.gameObject);
        }
        else if (other.CompareTag("Shield"))
        {
            other.GetComponent<ShieldPresenter>().isHeal = true;
            enemies.Remove(other.gameObject);
        }
        else if (other.CompareTag("Mino"))
        {
            other.GetComponent<MinoPresenter>().isHeal = true;
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
            // 메테리얼 색상 변경이 안됨
            // foreach(MeshRenderer mesh in meshRenderers)
            // {
            //     mesh.material.color = Color.red;
            // }
            //
            // yield return new WaitForSeconds(0.1f);
            //
            // foreach(MeshRenderer mesh in meshRenderers)
            // {
            //     mesh.material = _originalColors[mesh];
            // }
            _isHit = false;
        }
        
        yield return null;
    }
    
    private IEnumerator Dissolve()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
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
