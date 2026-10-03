using UnityEngine;
using UnityEngine.EventSystems;

// On-screen movement joystick for touch devices. The project had no touch
// input at all before this - PlayerMovement only ever read
// Input.GetAxisRaw("Horizontal"/"Vertical"), which sees a keyboard or a
// connected gamepad but never a finger on glass, so the game could not
// actually be played on a phone. A thumb press anywhere inside this
// object's own rect (the base) lets the player drag "handle" around; how
// far and in what direction it sits, clamped to handleRange, becomes
// Direction. PlayerMovement reads Direction in place of the keyboard axes
// whenever it is non-zero, and falls back to the keyboard otherwise - so
// testing in the Editor still works exactly as before.
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static VirtualJoystick Instance;

    public RectTransform handle;
    public float handleRange = 55f;

    public Vector2 Direction { get; private set; }

    private RectTransform baseRect;
    private Vector2 handleHomePosition;

    void Awake()
    {
        Instance = this;
        baseRect = GetComponent<RectTransform>();
        if (handle != null) handleHomePosition = handle.anchoredPosition;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Treat the initial touch-down the same as an immediate drag to
        // that point, so the handle jumps straight under the thumb instead
        // of waiting for the first move before responding.
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            return;
        }

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);
        if (handle != null) handle.anchoredPosition = handleHomePosition + clamped;
        Direction = clamped / handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        if (handle != null) handle.anchoredPosition = handleHomePosition;
    }
}
