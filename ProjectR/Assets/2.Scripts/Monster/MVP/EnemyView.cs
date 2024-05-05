using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyView : MonoBehaviour, IEnemyView
{
    public Image hpBar;
    private IEnemyPresenter _presenter;
    private Animator _animator;

    private void Start()
    {
        _presenter = new EnemyPresenter(this);
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }


    #region 이벤트 트리거 사회자 함수

    private void OnTriggerEnter(Collider other)
    {
        
    }

    #endregion
    
    #region 뷰 함수를 사회자를 통해 호출

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        
    }

    public void PlayAnimation(string name)
    {
        
    }
    

    #endregion
    
}
