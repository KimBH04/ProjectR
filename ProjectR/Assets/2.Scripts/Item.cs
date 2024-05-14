using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum  EType
    {
        Hp,
        Mp,
        Elixir,
        Shield,
        Exp,
        Bomb
    }
    
    public EType type;
    public float value;

    private Rigidbody _rb;


    private void Awake()
    {
        _rb= GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (type != EType.Bomb)
        {
            transform.Rotate(Vector3.up * 20 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case EType.Hp:
                    //other.GetComponent<PlayerController>().Hp += value;
                    Destroy(gameObject);
                    break;
                case EType.Mp:
                    //other.GetComponent<PlayerController>().Mp += value;
                    Destroy(gameObject);
                    break;
                case EType.Elixir:
                    //other.GetComponent<PlayerController>().Elixir += value;
                    Destroy(gameObject);
                    break;
                case EType.Shield:
                    //other.GetComponent<PlayerController>().Shield += value;
                    Destroy(gameObject);
                    break;
                case EType.Exp:
                    //other.GetComponent<PlayerController>().Exp += value;
                    Destroy(gameObject);
                    break;
                case EType.Bomb:
                    StartCoroutine(Explosion());
                    break;
                default:
                    print("????");
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
}
