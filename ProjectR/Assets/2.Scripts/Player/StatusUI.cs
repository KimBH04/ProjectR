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
    //[SerializeField] private RectTransform hpImage;
    [SerializeField] private Image hpBar;
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
        PlayerController.CanControl = true;
        propertiesPanel.SetActive(false);
    }

    #region Static methods
    public static void PopUpSelectProperties(int level)
    {
        if (instance == null) return;

        Time.timeScale = 0f;
        PlayerController.CanControl = false;
        instance.propertiesPanel.SetActive(true);
    }

    public static void SetExpUI(int exp, int needExp)
    {
        if (instance == null) return;

        instance.expBar.fillAmount = (float)exp / needExp;
        instance.expText.text = $"{exp} / {needExp}";
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
