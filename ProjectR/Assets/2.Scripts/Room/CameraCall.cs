using DG.Tweening;
using UnityEngine;

public class CameraCall : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;

    private void OnTriggerEnter(Collider other)
    {
        Camera.main.transform.DOMove(cameraPos.position, 0.5f).SetEase(Ease.Linear);
    }
}
