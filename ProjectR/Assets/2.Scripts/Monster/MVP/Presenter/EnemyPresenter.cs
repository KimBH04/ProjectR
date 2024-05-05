// Presenter
// MonoBehaviour
// Model의 직접 레퍼런스와 View의 인터페이스 레퍼런스를 가진다.
// Model과 View를 이벤트로 연결해준다.
// 다른 Presenter나 Model의 레퍼런스를 가지기도 한다.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 로직과 Model-View간의 통신을 담당 
// 본질적으로는 MVC의 컨트롤러와 같지만, 뷰에 연결되는 것이 아니라
// 그냥 인터페이스라는 점이 다르다.
// Presenter는 View에서 분리되어있다.
// View를 interface를 통해 조작
public class EnemyPresenter : MonoBehaviour
{
    public EnemyData data;
    private EnemyModel _model;
    private EnemyView _view;
    private Animator _animator;

    private void Awake()
    {
        _model = new EnemyModel(data.maxHp);
        _animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        
    }

    public void TakeDamage(float damage)
    {
        _model.currentHp -= damage;
        print(_model.currentHp);
        if (_model.currentHp < 0)
        {
            _model.currentHp = 0;
        }
        if(_model.currentHp > _model.maxHp)
        {
            _model.currentHp = _model.maxHp;
        }
        //view.UpdateHpBar(_model.currentHp,_model.maxHp);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skill"))
        {
            TakeDamage(10f);
        }
    }
}
