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
            GUILayout.Space(10);
            GUILayout.Label("Test");

            var inspector = (PlayerController)target;
            if (GUILayout.Button("Add 10 exp"))
            {
                inspector.Exp += 10;
            }

            if (GUILayout.Button("Heal"))
            {
                inspector.Hp++;
            }

            if (GUILayout.Button("Deal"))
            {
                inspector.Hp--;
            }

            if (GUILayout.Button("Can Skill"))
            {
                PlayerController.canSkill ^= true;
            }
        }
    }
}
#endif

public sealed class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int atk;
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float speed = 4f;
    [SerializeField, Min(0f)] private float rotSpeed = 10f;
    [SerializeField] private float maxStamina = 20f;
    [SerializeField] private float staminaSpeed = 1f;
    [Space]
    [SerializeField] private float dodgeForce;
    [SerializeField] private float dodgeCoolTime;

    // current status
    private int level = 1;
    private int exp = 0;
    private float stamina;
    private int hp;
    private bool isUnbeatable = false;
    private bool isBeaten = false;
    private bool isFootstep = false;

    private static bool canControl = true;
    public static bool canSkill = false;

    private float speedScale = 1f;

    private bool isDodge;
    private bool isDodgeCoolDown = true;
    private float rotScale = 1f;

    private static PlayerInput playerInput;
    private CharacterController controller;
    private float horizontal;
    private float vertical;

    private Vector3 lastFixedPos;
    private Vector3 nextFixedPos;

    private static PlayerAnimator pAnimator;

    [Header("Skills")]
    [SerializeField] private Skill[] skills;

    [Header("Objects")]
    [SerializeField] private Transform pointer;
    private Plane plane;

    #region Status properties
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
                exp -= NeedExp;
                level++;
                StatusUI.PopUpSelectProperties(level);
                AudioManager.Instance.PlaySfx(AudioManager.ESfx.PlayerLevelUp);
            }
            StatusUI.SetExpUI(exp, NeedExp, level);
        }
    }

    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            if (hp > value)
            {
                if (isUnbeatable || isBeaten)
                {
                    return;
                }
                StartCoroutine(Unbeatable());
            }

            hp = value;
            hp = Mathf.Clamp(hp, 0, maxHp);
           
            if (hp == 0)
            {
                CanControl = false;
                canSkill = false;
                AudioManager.Instance.PlaySfx(AudioManager.ESfx.PlayerDead);
                pAnimator.PlayDie();
            }
            StatusUI.SetHpUI(hp, maxHp);
        }
    }

    public int NeedExp => level * 50;

    public static bool CanControl
    {
        get
        {
            return canControl;
        }
        set
        {
            playerInput.enabled = canControl = canSkill = value;
        }
    }
    #endregion

    private void Awake()
    {
        stamina = maxStamina;
        hp = maxHp;

        playerInput = GetComponent<PlayerInput>();
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

        StatusUI.SetExpUI(0, NeedExp, 1);
        StatusUI.SetStaminaUI(stamina, maxStamina);
        StatusUI.SetHpUI(hp, maxHp);
    }

    private void Update()
    {
        Movement();

        stamina = Mathf.Min(maxStamina, stamina + Time.deltaTime * staminaSpeed);
        StatusUI.SetStaminaUI(stamina, maxStamina);
    }

    private void FixedUpdate()
    {
        lastFixedPos = nextFixedPos;
        nextFixedPos = speed * speedScale * Time.fixedDeltaTime * new Vector3(horizontal, controller.isGrounded ? 0f : -1f, vertical);
    }

    private void Movement()
    {
        if (Time.timeScale < float.Epsilon)
        {
            return;
        }

        plane = new Plane(Vector3.up, transform.position);

        if (!isDodge)
        {
            //movement
#pragma warning disable UNT0004 // Time.fixedDeltaTime used with Update
            float interpolation = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
#pragma warning restore UNT0004 // Time.fixedDeltaTime used with Update
            controller.Move(Vector3.Lerp(lastFixedPos, nextFixedPos, interpolation));

            if (horizontal != 0f || vertical != 0f)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(new Vector3(horizontal, 0f, vertical)),
                    rotScale * rotSpeed * Time.deltaTime);
            }
        }
    }

    #region New Input Systems
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanControl)
        {
            return;
        }

        Vector2 v2 = context.ReadValue<Vector2>();
        horizontal = v2.x;
        vertical = v2.y;

        pAnimator.SetMovementValue(0f, horizontal != 0f || vertical != 0f ? 1f : 0f);
        if(!isFootstep)
            StartCoroutine(PlayFootstepSound());
    }

    private IEnumerator PlayFootstepSound()
    {
        isFootstep = true;
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.PlayerFootstepStage1);
        yield return new WaitForSeconds(2.4f);
        isFootstep = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (canSkill && context.started)
        {
            int idx = (int)context.ReadValue<float>();
            if (idx < skills.Length && skills[idx].AttackCoolDown)
            {
                SkillContainer container = skills[idx].SkillObject.CurrentContainer;
                if (stamina >= container.NeedStamina)
                {
                    speedScale = 0.5f;
                    stamina -= container.NeedStamina;

                    foreach (var routine in skills[idx].GetDoSkillCoroutines())
                    {
                        StartCoroutine(routine);
                    }

                    pAnimator.PlayAttack(container.AnimationKey);
                }
            }
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
       
        if (context.started && isDodgeCoolDown && (horizontal != 0f || vertical != 0f))
        {
            AudioManager.Instance.PlaySfx(AudioManager.ESfx.PlayerRoll);
            isDodge = true;
            isDodgeCoolDown = false;
            rotScale = 0f;

            pAnimator.PlayDodge();

            StartCoroutine(Dodge(transform.position + (new Vector3(horizontal, 0f, vertical).normalized * dodgeForce), 0.3f));
            StartCoroutine(DodgeCoolTime());
        }
    }
    #endregion

    #region Coroutine Methods
    private IEnumerator Dodge(Vector3 endPos, float time)
    {
        transform.LookAt(endPos);
        isUnbeatable = true;

        Vector3 startPos = transform.position;

        // dodge
        float currentTime = 0f;
        while (currentTime < time)
        {
            controller.Move(Time.deltaTime / time * (endPos - startPos));
            currentTime += Time.deltaTime;

            yield return null;
        }

        isUnbeatable = false;

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
        
        // 구르기 끝났을 때 부드럽게 회전
        while (rotScale < 1f)
        {
            rotScale += Time.deltaTime * rotSpeed;

            yield return null;
        }
    }

    private IEnumerator DodgeCoolTime()
    {
        yield return new WaitForSeconds(dodgeCoolTime);
        isDodgeCoolDown = true;
    }

    private IEnumerator Unbeatable()
    {
        isBeaten = true;
        yield return new WaitForSeconds(1f);
        isBeaten = false;
    }
    #endregion

    [System.Serializable]
    private class Skill
    {
        [SerializeField] private SkillObject skillObject;

        [HideInInspector] public UnityEvent AttackEndedEvent;

        public SkillObject SkillObject => skillObject;

        public bool AttackCoolDown { get; private set; } = true;

        public IEnumerator[] GetDoSkillCoroutines()
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
                // AudioManager.Instance.PlaySfx(AudioManager.ESfx.Slash2);
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
