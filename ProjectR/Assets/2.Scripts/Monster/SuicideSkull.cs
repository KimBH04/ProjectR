using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class SuicideSkull : Enemy
{
    [SerializeField] 
    private GameObject explosionEffect;

    [SerializeField] private List<GameObject> bombs;
    
    public float radius=5f;
    
   

    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override IEnumerator AttackPlayer()
    {
        if (bombs.Count == 0)
        {
            yield break;
        }
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.SkeletonBombBeep);
        IsAttack = true;
        StartCoroutine(CountExplosion());
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.SkeletonBombExplosion);
        
        explosionEffect.SetActive(true);
        bombs[0].SetActive(false);
        bombs.Remove(bombs[0]);
        RaycastHit[] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,LayerMask.GetMask("Player","Enemy"));
        
        
        foreach (RaycastHit hitObj in rayHits)
        {
            if(hitObj.transform.CompareTag("Player"))
            {
                hitObj.transform.GetComponent<PlayerController>().Hp -= 2;
            }
            else if (hitObj.transform.CompareTag("Enemy"))
            {
                hitObj.transform.GetComponent<Enemy>().TakeDamage(50f);
            }
            
        }
        
        yield return new WaitForSeconds(1.5f);
        explosionEffect.SetActive(false);
        
        IsAttack = false;
    }
    
    private IEnumerator CountExplosion()
    {
        float[] intervals = { 0.3f, 0.2f,0.2f,0.2f,0.1f,0.1f,0.1f,0.05f,0.05f,0.05f,0.05f,0.05f,0.05f};
        foreach (float interval in intervals)
        {
            SetBombColor(Color.red);
            yield return new WaitForSeconds(interval);
            ResetBombColor();
            yield return new WaitForSeconds(interval);
        }
    }
    
    private void SetBombColor(Color color)
    {
        if(bombs.Count==0) 
            return;
        foreach (var mesh in bombs[0].GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mesh.material.color = color;
        }
    }

    private void ResetBombColor()
    {
        if(bombs.Count==0) 
            return;
        foreach (var mesh in bombs[0].GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
