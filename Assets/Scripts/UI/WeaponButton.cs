using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Weapon weapon;
    public UIManager uiManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiManager != null && weapon != null)
            uiManager.ShowTooltip(weapon, transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (uiManager != null)
            uiManager.HideTooltip();
    }
}
