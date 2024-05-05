using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리젠터는 모델과 뷰 사이의 중간자 역할
// 상호 작용하고 , 비즈니스 로직을 처리
public class EnemyPresenter : IEnemyPresenter
{
    private IEnemyView _view;
    private EnemyModel _model;

    public EnemyPresenter(IEnemyView view)
    {
        _model = new EnemyModel();
        _view = view;
    }
    public void Attack()
    {
        
    }

    public void TakeDamage(float damage)
    {
        
    }

    public void Die()
    {
        
    }
}
