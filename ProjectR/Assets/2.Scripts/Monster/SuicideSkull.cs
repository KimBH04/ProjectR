using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideSkull : Enemy
{
    [SerializeField] 
    private GameObject explosionEffect;
    
    protected override IEnumerator AttackPlayer()
    {

        StartCoroutine(CountExplosion());
        yield return new WaitForSeconds(3f);
        
        IsAttack = true;
        StartCoroutine(OnDie());
        explosionEffect.SetActive(true);
        RaycastHit[] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,LayerMask.GetMask("Player"));
        
        
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<PlayerController>().Hp-=2;
        }
        
        IsChase = false;
        Animator.SetBool(Attack,true);
        
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private IEnumerator CountExplosion()
    {
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }
        
        yield return new WaitForSeconds(0.3f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.3f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.2f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.2f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.2f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.2f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.1f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.05f);
        
        foreach (SkinnedMeshRenderer mesh in _meshRenderers)
        {
            mesh.material.color = _originalMeshRenderers[mesh];
        }   
        
        
    }

    private void OnDrawGizmos()
    {
        
        RaycastHit[] rayHits =Physics.SphereCastAll(transform.position,15,Vector3.up,0f,LayerMask.GetMask("Player"));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);

        // Draw lines to all hits
        Gizmos.color = Color.green;
        foreach (RaycastHit hit in rayHits)
        {
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }
}
