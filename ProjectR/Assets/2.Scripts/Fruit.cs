using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public enum EType
    {
        Bomb,
        Wind,
        Tingling
    }
    public EType type;
    public float value;
    public GameObject head;
    public GameObject effect;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case EType.Bomb:
                    StartCoroutine(Explosion());
                    break;
                case EType.Wind:
                    StartCoroutine(Wind());
                    break;
                case EType.Tingling:
                    StartCoroutine(Tingling());
                    break;
            }
        }
    }
    
    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(1f);
        head.SetActive(false);
        effect.SetActive(true);
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,layerMask);

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Enemy"))
            {
                hitObj.transform.GetComponent<Enemy>().TakeDamage(value);
            }
            
            else if (hitObj.transform.CompareTag("Player")) 
            {
                // hitObj.transform.GetComponent<PlayerController>().TakeDamage(10f);
            }
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private IEnumerator Wind()
    {
        head.SetActive(false);
        effect.SetActive(true);
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,layerMask);

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Enemy"))
            {
                hitObj.transform.GetComponent<Enemy>().SetSpeed(value);
                yield return new WaitForSeconds(10f);
                hitObj.transform.GetComponent<Enemy>().OriginSpeed(value);
            }
            else if (hitObj.transform.CompareTag("Player")) 
            {
                yield return new WaitForSeconds(10f);
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator Tingling()
    {
        head.SetActive(false);
        effect.SetActive(true);
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,layerMask);
        
        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Enemy"))
            {
                hitObj.transform.GetComponent<Enemy>().Tingling();
                
                yield return new WaitForSeconds(1f);
                hitObj.transform.GetComponent<Enemy>().EndTingling();
            }
            else if (hitObj.transform.CompareTag("Player")) 
            {
                yield return new WaitForSeconds(1f);
            }
        }
        Destroy(gameObject);
    }
    
    
    private void OnDrawGizmos()
    {
        RaycastHit[] rayHits =Physics.SphereCastAll(transform.position,5,Vector3.up,0f,LayerMask.GetMask("Player"));
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
