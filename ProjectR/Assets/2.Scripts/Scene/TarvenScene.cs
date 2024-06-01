using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarvenScene : MonoBehaviour
{
    public GameObject player;
    public Transform doorTr;
    private void Awake()
    {
       
            player = GameObject.FindWithTag("Player");
            player.transform.position = doorTr.position;
    }
}
