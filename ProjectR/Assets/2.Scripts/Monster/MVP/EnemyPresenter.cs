using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리젠터는 모델과 뷰 사이의 중간자 역할
// 상호 작용하고 , 비즈니스 로직을 처리
public class EnemyPresenter : MonoBehaviour
{
    private EnemyModel _model;
    private EnemyView _view;

    private void Awake()
    {
        _model = new EnemyModel(maxHp: 10f);
        _view = GetComponent<EnemyView>();
    }

    private void Start()
    {
        Invoke(nameof(ChaseStart),2f);
    }

    private void Update()
    {
        
    }

    private void ChaseStart()
    {
        
    }

    public void TakeDamage(float damage)
    {
        _model.TakeDamage(damage);
        _view.UpdateHpBar(_model.currentHp, _model.maxHp);
    }
}
