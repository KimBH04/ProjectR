using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap : MonoBehaviour
{
    [SerializeField] private GameObject thorn;
    [SerializeField] private Collider meleeArea;
    private bool _kanban;

    public ETrapType _trapType;
    public enum ETrapType
    {
        Thorn,
        Fire,
    }

    private void Start()
    {
        if (_trapType == ETrapType.Fire)
        {
            StartCoroutine(HaveFire());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Player")) && !_kanban && _trapType == ETrapType.Thorn)
        {
            StartCoroutine(HaveThorn());
        }
        
        if ((other.CompareTag("Enemy") || other.CompareTag("Player")) && !_kanban && _trapType == ETrapType.Fire)
        {
            StartCoroutine(HaveFire());
        }
    }

    private IEnumerator HaveThorn()
    {
        _kanban = true;
        yield return new WaitForSeconds(0.3f);
        thorn.transform.DOLocalMoveY(0f, 0.1f).SetEase(Ease.OutBounce);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;
        thorn.transform.DOLocalMoveY(-1.5f, 0.5f).SetEase(Ease.InBounce); // -1f 대신 초기 위치로 변경
        yield return new WaitForSeconds(0.5f);

        _kanban = false;
        
    }

    private IEnumerator HaveFire()
    {
        while (true)
        {
            _kanban = true;
            thorn.SetActive(true);
            thorn.transform.DOLocalMoveY(0.12f, 2f);
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(3f);
            thorn.transform.DOLocalMoveY(-5f, 2f);
            yield return new WaitForSeconds(1f);
            thorn.SetActive(false);
            _kanban = false;
            yield return new WaitForSeconds(3f);
        }
        
    }

    
}
