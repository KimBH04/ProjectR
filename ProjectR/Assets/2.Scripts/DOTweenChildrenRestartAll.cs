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
            enableTweens.AddListener(tween.DOPlay);
            disableTweens.AddListener(tween.DOComplete);
            disableTweens.AddListener(tween.RecreateTween);
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
