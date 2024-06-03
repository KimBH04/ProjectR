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

    private Transform _playerTr;
    
    public EType type;
    public int value;
    public float moveSpeed = 10f;
    public float attractRange = 10f;


    private void Awake()
    {
        if(type == EType.Exp)
            _playerTr = GameObject.FindWithTag("Player").transform;
        
    }

    private void Update()
    {
        if (type == EType.Exp)
        {
            float distance = Vector3.Distance(transform.position, _playerTr.position);
            if (distance < attractRange)
            {
                Vector3 direction = (_playerTr.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
            
            //transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySfx(AudioManager.ESfx.Heal);
            switch (type)
            {
                case EType.Hp:
                    other.GetComponent<PlayerController>().Hp += value;
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
