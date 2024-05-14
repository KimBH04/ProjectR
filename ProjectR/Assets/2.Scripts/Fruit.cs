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
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,15,Vector3.up,0f,layerMask);

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Warrior"))
            {
                hitObj.transform.GetComponent<WarriorPresenter>().TakeDamage(value);
            }
            else if (hitObj.transform.CompareTag("Archer"))
            {
                hitObj.transform.GetComponent<ArcherPresenter>().TakeDamage(value);
            }
            else if (hitObj.transform.CompareTag("Shield")) 
            {
                hitObj.transform.GetComponent<ShieldPresenter>().TakeDamage(value);
            }
            else if (hitObj.transform.CompareTag("Player")) 
            {
                // hitObj.transform.GetComponent<PlayerController>().TakeDamage(10f);
            }
        }
        Destroy(gameObject);
    }
    
    private IEnumerator Wind()
    {
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,15,Vector3.up,0f,layerMask);

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Warrior"))
            {
                hitObj.transform.GetComponent<WarriorPresenter>().SetSpeed(value);
                head.SetActive(false);
                yield return new WaitForSeconds(10f);
                hitObj.transform.GetComponent<WarriorPresenter>().OriginSpeed(value);
            }
            else if (hitObj.transform.CompareTag("Archer"))
            {
                hitObj.transform.GetComponent<ArcherPresenter>().SetSpeed(value);
                head.SetActive(false);
                yield return new WaitForSeconds(10f);
                hitObj.transform.GetComponent<ArcherPresenter>().OriginSpeed(value);
            }
            else if (hitObj.transform.CompareTag("Shield")) 
            {
                hitObj.transform.GetComponent<ShieldPresenter>().SetSpeed(value);
                head.SetActive(false);
                yield return new WaitForSeconds(10f);
                hitObj.transform.GetComponent<ShieldPresenter>().OriginSpeed(value);
            }
            else if (hitObj.transform.CompareTag("Player")) 
            {
                
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator Tingling()
    {
        
        int layerMask = LayerMask.GetMask("Enemy", "Player");
        RaycastHit [] rayHits =Physics.SphereCastAll(transform.position,15,Vector3.up,0f,layerMask);

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.CompareTag("Warrior"))
            {
                hitObj.transform.GetComponent<WarriorPresenter>().Tingling();
                head.SetActive(false);
                yield return new WaitForSeconds(1f);
                hitObj.transform.GetComponent<WarriorPresenter>().EndTingling();
            }
            else if (hitObj.transform.CompareTag("Archer"))
            {
                hitObj.transform.GetComponent<ArcherPresenter>().Tingling();
                head.SetActive(false);
                yield return new WaitForSeconds(1f);
                hitObj.transform.GetComponent<ArcherPresenter>().EndTingling();
            }
            else if (hitObj.transform.CompareTag("Shield")) 
            {
                hitObj.transform.GetComponent<ShieldPresenter>().Tingling();
                head.SetActive(false);
                yield return new WaitForSeconds(1f);
                hitObj.transform.GetComponent<ShieldPresenter>().EndTingling();
            }
            else if (hitObj.transform.CompareTag("Player")) 
            {
                
            }
        }
        Destroy(gameObject);
    }
}
