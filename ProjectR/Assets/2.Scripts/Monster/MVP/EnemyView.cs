using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 뷰는 정보를 보여주는 곳
// GameObject의 활성화 / 비활성화 동작을 담당 
public class EnemyView : MonoBehaviour
{
    public Image hpBar;

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
    }
}
