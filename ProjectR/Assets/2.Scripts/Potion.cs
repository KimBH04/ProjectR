using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public enum  EType
    {
        Hp,
        Mp,
        Elixir,
        Shield
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
                    break;
                case EType.Mp:
                    //other.GetComponent<PlayerController>().Mp += value;
                    break;
                case EType.Elixir:
                    //other.GetComponent<PlayerController>().Elixir += value;
                    break;
                case EType.Shield:
                    //other.GetComponent<PlayerController>().Shield += value;
                    break;
            }
        }
    }
}
