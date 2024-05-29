using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DOTweenChildrenRestartAll : MonoBehaviour
{
    private readonly UnityEvent enableTweens = new UnityEvent(),
                                disableTweens = new UnityEvent();

    [SerializeField, Tooltip("디버깅")] private DOTweenAnimation[] Tweens;

    private void Awake()
    {
        Tweens = GetComponentsInChildren<DOTweenAnimation>();
        foreach (var tween in Tweens)
        {
            enableTweens.AddListener(tween.DOPlay);         // 시작
            disableTweens.AddListener(tween.DOComplete);    // 즉시 완료
            disableTweens.AddListener(tween.RecreateTween); // 동작 재생성
        }
    }

    private void OnEnable()
    {
        enableTweens.Invoke();
    }

    private void OnDisable()
    {
        disableTweens.Invoke();
    }
}
