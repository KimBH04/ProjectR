using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    private static StatusUI instance;
    [Space]
    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text expText;
    [Space]
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text staminaText;
    [Space]
    [SerializeField] private RectTransform hpImage;
    [Space]
    [SerializeField] private GameObject propertiesPanel;

    private void Awake()
    {
        instance = this;
    }

    public static void SetExpUI(int exp, int needExp)
    {
        instance.expBar.fillAmount = (float)exp / needExp;
        instance.expText.text = $"{exp} / {needExp}";
    }

    public static void SetStaminaUI(float stamina, float maxStamina)
    {
        instance.staminaBar.fillAmount = stamina / maxStamina;
        instance.staminaText.text = $"{(int)stamina} / {maxStamina}";
    }

    public static void SetHpUI(int hp)
    {
        instance.hpImage.sizeDelta = new Vector2(hp * 50f, 100f);
    }

    public static void PopUpSelectProperties(int level)
    {
        Debug.Log("Level Up! Lv." + level);
        instance.propertiesPanel.SetActive(true);
    }
}
