using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    private static StatusUI instance;

    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text expText;

    private void Awake()
    {
        instance = this;
        expBar.fillAmount = 0f;
        expText.text = "0 / 50";
    }

    public static void SetExpUI(int exp, int needExp)
    {
        instance.expBar.fillAmount = (float)exp / needExp;
        instance.expText.text = $"{exp} / {needExp}";
    }

    public static void PopUpSelectProperties()
    {
        Debug.Log("Level Up!");
    }
}
