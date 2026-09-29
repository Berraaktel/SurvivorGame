using UnityEngine;

// Lightweight frame-swap + "juice" animator - no Animator Controller asset
// needed. Measures movement itself by comparing world position between
// FixedUpdate calls (the same cadence PlayerMovement/EnemyAI actually move
// on via Rigidbody2D.MovePosition), instead of sampling at Update()'s
// variable frame rate - sampling at the wrong cadence was a real source of
// false "not moving" reads right when movement starts/stops.
//
// Three things happen together while moving, so the animation stays
// visible even if any single one of them turns out to be too subtle on
// screen: the sprite swaps between the walk-frame sprites, the whole
// visual hops by a whole pixel (safe here since this lives on a separate
// child object with no collider of its own), AND the sprite tilts back
// and forth a few degrees. The tilt is the most important of the three -
// rotation is never affected by pixel-perfect position snapping, so it
// stays visible no matter what the camera/import settings end up being.
public class SpriteAnimator : MonoBehaviour
{
    public Sprite idleFrame;
    public Sprite[] moveFrames;
    public float frameRate = 8f;
    public float moveSpeedThreshold = 0.05f;
    public float hopPixels = 2f;
    public float pixelsPerUnit = 100f;
    public float tiltAngle = 8f;
    public float tiltSpeed = 14f;

    private SpriteRenderer sr;
    private Vector3 lastFixedPosition;
    private Vector3 basePosition;
    private bool isMoving;
    private float frameTimer;
    private int frameIndex;
    private float tiltTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        basePosition = transform.localPosition;
        if (idleFrame == null && sr != null) idleFrame = sr.sprite;
    }

    void OnEnable()
    {
        // Reset on reuse (pooled enemies) so a recycled object doesn't
        // start mid-walk-cycle or measure a huge "speed" from teleporting
        // to its new spawn position.
        lastFixedPosition = transform.position;
        isMoving = false;
        frameTimer = 0f;
        frameIndex = 0;
        tiltTimer = 0f;
        transform.localPosition = basePosition;
        transform.localRotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        float speed = (transform.position - lastFixedPosition).magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        lastFixedPosition = transform.position;
        // Note: this does NOT require moveFrames to be set. Some
        // creatures (enemies, right now) only have a single sprite per
        // species rather than a drawn walk cycle - for those, moveFrames
        // is left empty and the tilt/hop below is the only motion cue,
        // which still reads fine on its own.
        isMoving = speed > moveSpeedThreshold;
    }

    void Update()
    {
        if (sr == null) return;

        if (!isMoving)
        {
            frameTimer = 0f;
            frameIndex = 0;
            tiltTimer = 0f;
            transform.localPosition = basePosition;
            transform.localRotation = Quaternion.identity;
            if (idleFrame != null) sr.sprite = idleFrame;
            return;
        }

        bool hasFrames = moveFrames != null && moveFrames.Length > 0;
        int hopParity = 0;

        if (hasFrames)
        {
            float frameDuration = 1f / Mathf.Max(frameRate, 0.01f);
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameDuration)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % moveFrames.Length;
            }

            sr.sprite = moveFrames[frameIndex];
            hopParity = frameIndex % 2;
        }
        else
        {
            // No dedicated walk-cycle sprites for this creature - still
            // hop on a steady timer so the tilt below isn't the only cue.
            float frameDuration = 1f / Mathf.Max(frameRate, 0.01f);
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameDuration)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % 2;
            }
            hopParity = frameIndex % 2;
        }

        // Alternate between resting height and +hopPixels every other
        // frame in the cycle, so it reads as a clear up/down step bounce.
        float hopUnits = (hopParity == 0) ? 0f : hopPixels / Mathf.Max(pixelsPerUnit, 1f);
        transform.localPosition = basePosition + new Vector3(0f, hopUnits, 0f);

        // Continuous rocking tilt - independent of pixel snapping, always
        // visible while moving.
        tiltTimer += Time.deltaTime * tiltSpeed;
        float tilt = Mathf.Sin(tiltTimer) * tiltAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
    }
}
