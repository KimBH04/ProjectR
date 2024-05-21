using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap : MonoBehaviour
{
    [SerializeField] private GameObject thorn;
    private bool _kanban;
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Enemy") || 
             other.CompareTag("Player")) && 
            !_kanban)
        {
            StartCoroutine(HaveThorn());
        }
    }

    private IEnumerator HaveThorn()
    {
        _kanban = true;
        yield return new WaitForSeconds(0.3f);
        thorn.transform.DOLocalMoveY(0f, 0.1f).SetEase(Ease.OutBounce);
        
        yield return new WaitForSeconds(3f);
        
        thorn.transform.DOLocalMoveY(-1.5f, 0.5f).SetEase(Ease.InBounce); // -1f 대신 초기 위치로 변경
        yield return new WaitForSeconds(0.5f);

        _kanban = false;
        
    }
}
