using UnityEngine;

public class FallingTarget : MonoBehaviour
{
    // Configs
    private float decayTime = 10;
    private float existing_timer = 0f;

    // Component References
    private SpriteRenderer spriteRenderer;
    private Vector2 boxCenter;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCenter = transform.position;

        // Load game difficulty config
        GameInitConfig config = Helper.LoadGameInitConfig();
        decayTime = config.selfDestroyTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Update parameter based on controller input
        decayTime = ControllerListener.Instance.selfDestroyTime;
        existing_timer += Time.deltaTime;

        // Forcibly destroy this object if it exists for too long
        if (existing_timer > decayTime) {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        ScoreManager.Instance.AddMiss(); // Decrement user score.
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name != "FallingTarget(Clone)" && collision.collider.gameObject.name != "FallingTarget")
        {
            Destroy(gameObject);
            ScoreManager.Instance.AddScore();
        }
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
            if (hitCollider.gameObject.name != "FallingTarget(Clone)" && hitCollider.gameObject.name != "FallingTarget") return true;
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
