using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : Monster
{
    private enum EState
    {
        Idle,
        Move,
        Attack,
        Die
    }
    
    private EState _curState;
    private FSM _fsm;

    private void Start()
    {
        _curState = EState.Idle;
        _fsm = new FSM(new IdleState(this));
    }

    private void Update()
    {
        switch (_curState)
        {
            case EState.Idle:
                if (CanSeePlayer())
                {
                    if(CanAttackPlayer())
                        ChangeState(EState.Attack);
                    else
                        ChangeState(EState.Move);
                }
                break;
            case EState.Move:
                if (CanSeePlayer())
                {
                    if (CanAttackPlayer())
                    {
                        ChangeState(EState.Attack);
                    }
                }
                else
                {
                    ChangeState(EState.Idle);
                }
                break;
            case EState.Attack:
                if (CanSeePlayer())
                {
                    if (!CanAttackPlayer())
                    {
                        ChangeState(EState.Move);
                    }
                }
                else
                {
                    ChangeState(EState.Idle);
                }
                break;
        }
        _fsm.UpdateState();
    }

    private void ChangeState(EState nextState)
    {
        _curState = nextState;
        switch (_curState)
        {
            case EState.Idle:
                _fsm.ChangeState(new IdleState(this));
                break;
            case EState.Move:
                _fsm.ChangeState(new MoveState(this));
                break;
            case EState.Attack:
                _fsm.ChangeState(new AttackState(this));
                break;
            case EState.Die:
                _fsm.ChangeState(new DieState(this));
                break;
        }
    }

    
    // 미완성
    private bool CanSeePlayer()
    {
        
        // 플레이어 탐지 
        return true;
    }

    private bool  CanAttackPlayer()
    {
        // 사정 거리 체크
        return true;
    }
    
}
