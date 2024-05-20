using UnityEngine;
using UnityEngine.Events;

public /* abstract */ class Enemy : MonoBehaviour
{
    public UnityEvent onDieEvent = new UnityEvent();

    // 임시로 넣어둠
    private void OnDisable()
    {
        onDieEvent.Invoke();
    }
}
