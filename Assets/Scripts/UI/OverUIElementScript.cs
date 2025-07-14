using UnityEngine;
using UnityEngine.EventSystems;

public class OverUIElementScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.menuOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.menuOver = GameManager.Instance.menuEntered;
    }
}
