using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int level;
    [SerializeField] private int atk;
    [SerializeField] private int hp;
    [SerializeField] private float speed;

    private CharacterController controller;
    private float horizontal;
    private float vertical;

    private Vector3 lastFixedPos;
    private Vector3 nextFixedPos;

    private PlayerAnimator pAnimator;

    [Header("Skills")]
    [SerializeField] private Skill[] skills;

    [Header("Objects")]
    [SerializeField] private Transform pointer;
    private Plane plane;

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        pAnimator = GetComponentInChildren<PlayerAnimator>();

        plane = new Plane(transform.up, transform.position);
    }

    private void Update()
    {
        //movement
#pragma warning disable UNT0004 // Time.fixedDeltaTime used with Update
        float interpolation = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
#pragma warning restore UNT0004 // Time.fixedDeltaTime used with Update
        controller.Move(Vector3.Lerp(lastFixedPos, nextFixedPos, interpolation));

        //rotation
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);
            transform.LookAt(point);

            Vector3 normal = (point - transform.position).normalized;
            Vector3 nonInterMove = new Vector3(horizontal, 0f, vertical);
            float dot = Vector3.Dot(normal, nonInterMove);

            pAnimator.SetMovementValue(Vector3.Cross(normal, nonInterMove).y, dot);

            point.y += 0.1f;
            pointer.position = point;
        }
    }

    private void FixedUpdate()
    {
        lastFixedPos = nextFixedPos;
        nextFixedPos = speed * Time.fixedDeltaTime * new Vector3(horizontal, controller.isGrounded ? 0f : -1f, vertical);
    }

    #region New Input Systems
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 v2 = context.ReadValue<Vector2>();
        horizontal = v2.x;
        vertical = v2.y;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int idx = (int)context.ReadValue<float>();
            if (idx < skills.Length && skills[idx].AttackCoolDown)
            {
                var routines = skills[idx].GetDoSkill(transform);
                StartCoroutine(routines[0]);
                StartCoroutine(routines[1]);

                pAnimator.PlayAttack(skills[idx].SkillObject.CurrentContainer.AnimationKey);
            }
        }
    }
    #endregion

    [System.Serializable]
    private class Skill
    {
        [SerializeField] private SkillObject skillObject;
        [SerializeField] private Vector3 colliderScale;

        private bool attackCoolDown = true;

        public SkillObject SkillObject => skillObject;

        public bool AttackCoolDown => attackCoolDown;

        public IEnumerator[] GetDoSkill(Transform tr)
        {
            return new IEnumerator[]
            {
                CoolDown(),
                skillObject.PlaySkill(tr)
            };
        }

        private IEnumerator CoolDown()
        {
            attackCoolDown = false;
            yield return new WaitForSeconds(skillObject.CurrentContainer.CoolTime);
            attackCoolDown = true;
        }
    }
}
