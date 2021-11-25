using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public string ButtonName;
    public bool isPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
