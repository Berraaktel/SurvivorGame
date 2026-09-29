using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    public static CameraFollow Instance;

    private float baseZ;
    private Vector3 basePosition;
    private Vector3 shakeOffset;
    private float shakeTimeRemaining;
    private float shakeDuration;
    private float shakeMagnitude;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
        baseZ = transform.position.z;
        basePosition = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, baseZ);
        basePosition = Vector3.Lerp(basePosition, desiredPosition, smoothSpeed * Time.deltaTime);

        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.deltaTime;
            float t = Mathf.Clamp01(shakeTimeRemaining / shakeDuration);
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeMagnitude * t);
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        transform.position = new Vector3(basePosition.x + shakeOffset.x, basePosition.y + shakeOffset.y, baseZ);
    }

    // Quick camera shake for hit feedback - duration in seconds, magnitude
    // in world units. Safe to call repeatedly; a new call simply overrides
    // whatever shake is already in progress rather than stacking.
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeTimeRemaining = duration;
        shakeMagnitude = magnitude;
    }
}
