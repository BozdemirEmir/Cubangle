using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform joystickContainer;
    private RectTransform joystickHandle;

    private Vector2 joystickInput = Vector2.zero;

    public float maxRadius = 50f;

    private void Start()
    {
        joystickContainer = GetComponent<RectTransform>();
        joystickHandle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickInput = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickContainer, eventData.position, eventData.pressEventCamera, out localPosition))
        {
            float radius = maxRadius;
            Vector2 direction = localPosition - joystickContainer.rect.center;
            if (direction.magnitude > radius)
            {
                direction = direction.normalized * radius;
            }
            joystickHandle.anchoredPosition = direction;
            joystickInput = direction / radius;
        }
    }

    public Vector2 GetInput()
    {
        return joystickInput;
    }
}

