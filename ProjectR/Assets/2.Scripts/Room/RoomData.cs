using UnityEngine;

public class RoomData : MonoBehaviour
{
    public int depth = 0;

    [SerializeField] private Transform cameraPos;

    public Vector3 CamPos => cameraPos.position;
}