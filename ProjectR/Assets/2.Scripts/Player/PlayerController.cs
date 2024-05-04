using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    private CharacterController controller;
    private float horizontal;
    private float vertical;

    private Vector3 lastFixedPos;
    private Vector3 nextFixedPos;

    private void Awake()
    {
        _ = GameManager.Instance;
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
#pragma warning disable UNT0004 // Time.fixedDeltaTime used with Update
        float interpolation = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
#pragma warning restore UNT0004 // Time.fixedDeltaTime used with Update
        controller.Move(Vector3.Lerp(lastFixedPos, nextFixedPos, interpolation));
    }

    private void FixedUpdate()
    {
        lastFixedPos = nextFixedPos;
        nextFixedPos = speed * Time.fixedDeltaTime * new Vector3(horizontal, controller.isGrounded ? 0f : -1f, vertical);
    }

    #region New Input Systems
#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private void OnMove(InputValue value)
    {
        Vector2 v2 = value.Get<Vector2>();
        horizontal = v2.x;
        vertical = v2.y;
    }
#pragma warning restore IDE0051 // 사용되지 않는 private 멤버 제거
    #endregion
}
