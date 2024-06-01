using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnMouseEnter : MonoBehaviour , IPointerEnterHandler
{
   public void OnPointerEnter(PointerEventData eventData)
   {
      AudioManager.Instance.PlaySfx(AudioManager.ESfx.ButtonHover);
   }
}
