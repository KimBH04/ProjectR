using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DOTweenChildrenRestartAll : MonoBehaviour
{
    private readonly UnityEvent enableTweens = new UnityEvent(),
                                disableTweens = new UnityEvent();

    private DOTweenAnimation[] Tweens;

    private void Awake()
    {
        Tweens = GetComponentsInChildren<DOTweenAnimation>();
        foreach (var tween in Tweens)
        {
            tween.autoPlay = false;
            tween.autoKill = false;
            enableTweens.AddListener(tween.DOPlay);
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
