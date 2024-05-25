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
    }
    
    public EType type;
    public int value;

    private Rigidbody _rb;


    private void Awake()
    {
        _rb= GetComponent<Rigidbody>();
    }

    private void Update()
    {
            transform.Rotate(Vector3.up * 20 * Time.deltaTime);
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
                    other.GetComponent<PlayerController>().Exp += value;
                    Destroy(gameObject);
                    break;
                default:
                    print("??? : " + type + "아이템이 없습니다.");
                    break;
            }
        }
    }

    
}
