using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            var instpector = (PlayerController)target;
            if (GUILayout.Button("Add 10 exp"))
            {
                instpector.Exp += 10;
            }
        }
    }
}
#endif

public sealed class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int atk;
    [SerializeField] private int hp = 3;
    [SerializeField] private float speed = 4f;
    [Space]
    [SerializeField] private float dodgeForce;
    [SerializeField] private float dodgeCoolTime;

    private int level = 1;
    private bool didLevelUp = false;
    private int exp = 0;

    private float speedScale = 1f;

    private bool isDodge;
    private bool isDodgeCoolDown = true;
    private float rotationScale = 1f;
    private Quaternion dodgeRotate;

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

    public int Exp
    {
        get
        {
            return exp;
        }
        set
        {
            exp = value;
            if (exp >= NeedExp)
            {
                level++;
                didLevelUp = true;
            }
            StatusUI.SetExpUI(exp, NeedExp);
        }
    }

    public int NeedExp => (level + 1) * level * 25;

    private void Awake()
    {
        RoomData.roomClearEvent.AddListener(() =>
        {
            if (didLevelUp)
            {
                StatusUI.PopUpSelectProperties();
                didLevelUp = false;
            }
        });

        controller = GetComponent<CharacterController>();

        foreach (var skill in skills)
        {
            skill.SkillObject.Init();
            skill.AttackEndedEvent.AddListener(() =>
            {
                speedScale = 1f;
            });
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        pAnimator = GetComponentInChildren<PlayerAnimator>();
    }

    private void Update()
    {
        plane = new Plane(Vector3.up, transform.position);

        if (!isDodge)
        {
            //movement
#pragma warning disable UNT0004 // Time.fixedDeltaTime used with Update
            float interpolation = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
#pragma warning restore UNT0004 // Time.fixedDeltaTime used with Update
            controller.Move(Vector3.Lerp(lastFixedPos, nextFixedPos, interpolation));
        }

        //rotation
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);

            if (!isDodge)
            {
                Quaternion rotation = Quaternion.LookRotation(point - transform.position);
                transform.rotation = Quaternion.Lerp(dodgeRotate, rotation, rotationScale);

                Vector3 normal = (point - transform.position).normalized;
                Vector3 nonInterMove = new Vector3(horizontal, 0f, vertical);
                float dot = Vector3.Dot(normal, nonInterMove);

                pAnimator.SetMovementValue(Vector3.Cross(normal, nonInterMove).y, dot);
            }

            point.y += 0.1f;
            pointer.position = point;
        }
    }

    private void FixedUpdate()
    {
        lastFixedPos = nextFixedPos;
        nextFixedPos = speed * speedScale * Time.fixedDeltaTime * new Vector3(horizontal, controller.isGrounded ? 0f : -1f, vertical);
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
                speedScale = 0.5f;

                foreach (var routine in skills[idx].GetDoSkill())
                {
                    StartCoroutine(routine);
                }

                pAnimator.PlayAttack(skills[idx].SkillObject.CurrentContainer.AnimationKey);
            }
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.started && isDodgeCoolDown && (horizontal != 0f || vertical != 0f))
        {
            isDodge = true;
            isDodgeCoolDown = false;
            rotationScale = 0f;

            pAnimator.PlayDodge();

            StartCoroutine(Dodge(transform.position + (new Vector3(horizontal, 0f, vertical).normalized * dodgeForce), 0.3f));
            StartCoroutine(DodgeCoolTime());
        }
    }
    #endregion

    private IEnumerator Dodge(Vector3 endPos, float time)
    {
        transform.LookAt(endPos);
        dodgeRotate = transform.rotation;

        Vector3 startPos = transform.position;

        // dodge
        float currentTime = 0f;
        while (currentTime < time)
        {
            controller.Move(Time.deltaTime / time * (endPos - startPos));
            currentTime += Time.deltaTime;

            yield return null;
        }

        // roll
        var (newStartPos, newEndPos) = (endPos, (endPos - startPos).normalized * 3f + endPos);
        currentTime = 0f;
        while (currentTime < 0.5f)
        {
            controller.Move(4f * Time.deltaTime * (newEndPos - newStartPos));
            currentTime += Time.deltaTime;

            yield return null;
        }

        isDodge = false;
        
        // 구르기 끝났을 때 마우스 부드럽게 바라보기
        while (rotationScale < 1f)
        {
            rotationScale += Time.deltaTime * 3f;

            yield return null;
        }
    }

    private IEnumerator DodgeCoolTime()
    {
        yield return new WaitForSeconds(dodgeCoolTime);
        isDodgeCoolDown = true;
    }

    [System.Serializable]
    private class Skill
    {
        [SerializeField] private SkillObject skillObject;

        [HideInInspector] public UnityEvent AttackEndedEvent;

        public SkillObject SkillObject => skillObject;

        public bool AttackCoolDown { get; private set; } = true;

        public IEnumerator[] GetDoSkill()
        {
            if (skillObject is ComboContainer)
            {
                return new IEnumerator[]
                {
                    CoolDown(),
                    skillObject.CurrentContainer.PlaySkill(), // 콤보 요소
                    skillObject.PlaySkill()                   // 콤보
                };
            }
            else
            {
                return new IEnumerator[]
                {
                    CoolDown(),
                    skillObject.PlaySkill()                   // 단일 스킬
                };
            }
        }

        private IEnumerator CoolDown()
        {
            AttackCoolDown = false;
            yield return new WaitForSeconds(skillObject.CurrentContainer.CoolTime);
            AttackCoolDown = true;
            AttackEndedEvent.Invoke();
        }
    }
}
