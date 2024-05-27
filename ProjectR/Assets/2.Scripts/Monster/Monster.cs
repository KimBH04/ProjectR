using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    
    private enum EState
    {
        Idle,
        Move,
        Attack,
        Die
    }
    private EState _state;

    private void Start()
    {
        _state = EState.Idle;
    }

    private void Update()
    {
        switch (_state)
        {
            case EState.Idle:
                // Idle
                break;

            case EState.Move:
                // Move
                break;

            case EState.Attack:
                // Attack
                break;

            case EState.Die:
                // Die
                break;
        }
    }
}
