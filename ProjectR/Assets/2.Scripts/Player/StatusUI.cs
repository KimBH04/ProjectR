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

    private (bool stat, bool trait) isClicked = (false, false);

    private void Awake()
    {
        instance = this;
    }

    public void StatClick(int index)
    {
        Debug.Log("Clicked stat : " + index);
        isClicked.stat = true;

        if (isClicked.stat && isClicked.trait)
        {
            CloseProperties();
        }
    }

    public void TraitClick(int index)
    {
        Debug.Log("Clicked trait : " + index);
        isClicked.trait = true;

        if (isClicked.stat && isClicked.trait)
        {
            CloseProperties();
        }
    }

    private void CloseProperties()
    {
        isClicked = (false, false);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    #region Static methods
    public static void PopUpSelectProperties(int level)
    {
        Debug.Log("Level Up! Lv." + level);
        instance.propertiesPanel.SetActive(true);
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
        Time.timeScale = 0f;
        instance.hpImage.sizeDelta = new Vector2(hp * 50f, 100f);
    }
    #endregion
}
