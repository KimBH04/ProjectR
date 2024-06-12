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
            Destroy(gameObject);
        }
    }
}
