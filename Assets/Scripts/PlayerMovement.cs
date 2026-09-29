using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // The visible sprite may live on a child "SpriteVisual" object (see
        // BuildGameUITool's player animation setup) with the root's own
        // SpriteRenderer disabled, or - before that's been set up - directly
        // on this object. Prefer the child if it exists so flipping always
        // affects whichever SpriteRenderer is actually rendering.
        Transform visual = transform.Find("SpriteVisual");
        sr = visual != null ? visual.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Face the sprite toward whichever horizontal direction is being
        // pressed. Without this, pressing A still moved the character left
        // but the sprite kept its default (right-facing) look, which reads
        // as walking backwards.
        if (sr != null)
        {
            if (moveInput.x > 0.01f) sr.flipX = false;
            else if (moveInput.x < -0.01f) sr.flipX = true;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
