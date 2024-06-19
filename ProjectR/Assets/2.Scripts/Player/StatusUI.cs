using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusUI : MonoBehaviour
{
    private static StatusUI instance;
    [Space]
    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;
    [Space]
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text staminaText;
    [Space]
    //[SerializeField] private RectTransform hpImage;
    [SerializeField] private Image hpBar;
    [Space]
    [SerializeField] private GameObject propertiesPanel;

    [Header("Stats bar")]
    [SerializeField] private Image atkDamageBarImage;
    [SerializeField] private Image atkSpeedBarImage;
    [SerializeField] private Image staminaBarImage;
    private int atkDamageBarValue = 0;
    private int atkSpeedBarValue = 0;
    private int staminaBarValue = 0;

    private const float MAX_VALUE = 5f;

    private PlayerController _playerController;

    private (bool stat, bool trait) isClicked = (false, false);
    private (int stat, int trait) indexes;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    public void StatClick(int index)
    {
        indexes.stat = index;
        isClicked.stat = true;

        if (isClicked.stat && isClicked.trait)
        {
            CloseProperties();
        }
    }

    public void TraitClick(int index)
    {
        indexes.trait = index;
        isClicked.trait = true;

        if (isClicked.stat && isClicked.trait)
        {
            CloseProperties();
        }
    }

    private void CloseProperties()
    {
        switch (indexes.stat)
        {
            case 0:
                if (atkDamageBarValue >= MAX_VALUE)
                {
                    return;
                }

                atkDamageBarImage.fillAmount = ++atkDamageBarValue / MAX_VALUE;

                _playerController.additionalAtk += 3;
                break;

            case 1:
                if (atkSpeedBarValue >= MAX_VALUE)
                {
                    return;
                }

                atkSpeedBarImage.fillAmount = ++atkSpeedBarValue / MAX_VALUE;

                _playerController.additionalAtkSpeed += 2;
                break;

            case 2:
                if (staminaBarValue >= MAX_VALUE)
                {
                    return;
                }

                staminaBarImage.fillAmount = ++staminaBarValue / MAX_VALUE;

                _playerController.additionalDefaultStamina += 5;
                _playerController.additionalStaminaSpeed += 2;
                break;

            default:
                Debug.LogError($"Unassigned Index : {indexes.stat}");
                break;
        }
        
        switch (indexes.trait)
        {
            default:
                Debug.LogWarning("Trait features not implemented");
                break;
        }

        isClicked = (false, false);

        PlayerController.CanControl = true;
        propertiesPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    #region Static methods
    public static void PopUpSelectProperties(int level)
    {
        if (instance == null) return;

        // Time.timeScale = 0f;
        // PlayerController.CanControl = false;
        instance.propertiesPanel.SetActive(true);

        instance.transform.DOScale(Vector3.one, 1f).SetUpdate(true).OnComplete(() =>
        {
            Time.timeScale = 0f;
            PlayerController.CanControl = false;
        });

    }

    public static void SetExpUI(int exp, int needExp, int level)
    {
        if (instance == null) return;

        instance.expBar.fillAmount = (float)exp / needExp;
        instance.expText.text = $"{exp} / {needExp}";
        instance.levelText.text = $"Lv. {level}";
    }

    public static void SetStaminaUI(float stamina, float maxStamina)
    {
        if (instance == null) return;

        instance.staminaBar.fillAmount = stamina / maxStamina;
        instance.staminaText.text = $"{(int)stamina} / {maxStamina}";
    }

    public static void SetHpUI(int hp, int maxHp)
    {
        if (instance == null) return;

        //instance.hpImage.sizeDelta = new Vector2(hp * 50f, 100f);
        instance.hpBar.fillAmount = (float)hp / maxHp;
    }
    #endregion
}
