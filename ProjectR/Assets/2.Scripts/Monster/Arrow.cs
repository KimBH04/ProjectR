using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 2.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어에게 닿았다!");
            
            Destroy(gameObject);
        }
    }
}