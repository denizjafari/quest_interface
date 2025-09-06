using UnityEngine;

public class DisappearAfterTime : MonoBehaviour
{
    [Header("Gameplay Parameters")]
    [SerializeField] float collisionTime = 1f; // How much time a player should be hovering over this target.
    [SerializeField] float warningFlashingTime = 0.2f;

    // Controller Input Config
    private float selfDestroyTime; // How much time before this game object disappears. (a game object)
    private float warningTime; // How much time before this game object starts displaying warning. (a game object)

    // Timers
    private float timer = 0f; // How much time cursor overlaps with target.
    private float existing_timer = 0f; // How many seconds target has existed and not overlapped with cursor.

    // Component References
    private SpriteRenderer spriteRenderer;
    private Vector2 boxCenter; // Stores the position on this game object.

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component at the start
        boxCenter = transform.position; // Store the position of this game object into its variable.

        // Load game difficulty config
        GameInitConfig config = Helper.LoadGameInitConfig();
        selfDestroyTime = config.selfDestroyTime;
        warningTime = config.warningTime;
    }

    void Update()
    {
        // Update parameter based on controller input
        selfDestroyTime = ControllerListener.Instance.selfDestroyTime;
        warningTime = ControllerListener.Instance.warningTime;

        if (IsCursorOverlappingTarget())
        {
            timer += Time.deltaTime;
            ReduceSpriteAlpha();

            // User scores a point if the cursor overlaps for a long enough time.
            if (timer > collisionTime) 
            {
                ScoreManager.Instance.AddScore();
                Destroy(gameObject);
            }
        }
        else
        {
            ResetSpriteOpacity();
            timer = 0;
            existing_timer += Time.deltaTime; // By design, we only increment this timer when the user fails to overlap with the target.

            // Destroy this target if it exists for more than selfDestroyTime.
            if (existing_timer > selfDestroyTime)
            {
                ScoreManager.Instance.AddMiss(); // Decrement user score.
                Destroy(gameObject);
            }
            // If this game object has existed for more than warningTime, display warning.
            else if (existing_timer > warningTime) 
            {
                FlashingWarning();
            }
        }
    }


    /// <summary>
    ///   Displays flashing warning by updating the sprite's opacity.
    /// </summary>
    void FlashingWarning()
    {
        if (spriteRenderer == null) return;

        if ((int)(existing_timer / warningFlashingTime) % 2 == 0)
        {
            // Set alpha to 0 to make the sprite completely invisible
            Color color = spriteRenderer.color;
            color.a = 0f;  
            spriteRenderer.color = color;
        }
        else
        {
            // Set alpha to 1 to make the sprite completely visible
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    /// <summary>
    ///     Slowly decreases the target visibility as player is reaching for the game object.
    /// </summary>
    void ReduceSpriteAlpha()
    {
        if (spriteRenderer == null) return; // By design, fail quietly.

        // Slowly decrease visibility.
        Color color = spriteRenderer.color;
        float fadeSpeed = 1f / collisionTime * Time.deltaTime;
        color.a -= fadeSpeed; // Reduce alpha value to fade out
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Sets this game object to fully visible
    /// </summary>
    void ResetSpriteOpacity()
    {
        if (spriteRenderer == null) return; // By design, fail quietly.

        // Reset alpha value to fully opaque
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    /// <summary>
    ///     Checks if there's any other game objects overlapping with current target object.
    /// </summary>
    bool IsCursorOverlappingTarget()
    {
        float angle = 0f;
        Vector2 boxSize = GetGameObjectSize();

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.name != "Target(Clone)" && hitCollider.gameObject.name != "Target") return true;
        }
        return false;
    }

    /// <summary>
    ///     Obtain Target's Collider2D and return its size.
    /// </summary>
    /// <returns>A Vector2 object that represents size of the collider based on scaling_factor.</returns>
    Vector2 GetGameObjectSize()
    {
        Collider2D collider = GetComponent<Collider2D>();
        Renderer renderer = GetComponent<Renderer>();
        if (collider != null)
        {
            Vector2 boxSize = collider.bounds.size;
            return boxSize;
        }
        else if (renderer != null)
        {
            return renderer.bounds.size;
        }
        else
        {
            return new Vector2(1, 1);
        }
    }
}
