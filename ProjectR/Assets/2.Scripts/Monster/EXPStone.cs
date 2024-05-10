using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPStone : MonoBehaviour
{
    public float expAmount;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerController>().Exp += expAmount;
        }
        
    }
}
