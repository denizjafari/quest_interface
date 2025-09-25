using UnityEngine;
using System;
using System.Collections;

public class FallingTarget : MonoBehaviour
{
    // Configs
    private float decayTime = 10;
    private float existing_timer = 0f;
    private bool isHeld = false;
    private GameObject cursor;
    private GameObject basket;

    // Component References
    private SpriteRenderer spriteRenderer;
    private Vector2 boxCenter;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCenter = transform.position;
        cursor = GameObject.Find("Cursor").gameObject;
        basket = GameObject.Find("Basket").gameObject;

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

        // Follow the cursor if player grabbed the apple
        if (isHeld)
        {
            transform.position = cursor.transform.position;

            // Let the object fall into the basket.
            if (IsTargetOverBasket())
            {
                isHeld = false;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
        }
    }

    void OnBecameInvisible()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddMiss(); // Decrement user score.
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name == "Cursor" && !IsTargetOverBasket())
        {
            // The player "grabs" the apple, and this object should follow the cursor.
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.position = collision.transform.position;
            isHeld = true;
        }
        else if (collision.collider.gameObject.name == "Basket")
        {
            ScoreManager.Instance.AddScore();
            Destroy(gameObject);
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

    /// <summary>
    ///     Check and return a bool if this object's position is hovering over the basket.
    /// </summary>
    bool IsTargetOverBasket()
    {
        Collider2D collider = GetComponent<Collider2D>(), basketCollider = basket.GetComponent<Collider2D>();
        float xCenter = collider.bounds.center.x, xMin = basketCollider.bounds.min.x, xMax = basketCollider.bounds.max.x;

        // Add a bit of buffer space for it to be easier to drop the apple into the basket
        return xCenter < xMax - 0.2 && xCenter > xMin + 0.2;
    }

    /// <summary>
    /// Fades in the target, automatically starting the apple to be invisible.
    /// </summary>
    /// <param name="fadeDuration">The seconds to fade the target in.</param>
    /// <returns></returns>
    public IEnumerator FadeIn(float fadeDuration)
    {   
        Renderer renderer = GetComponent<Renderer>();
        Color finalColor = renderer.material.color;
        Color initalColor = new Color(finalColor.r, finalColor.g, finalColor.b, 0f);

        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            renderer.material.color = Color.Lerp(initalColor, finalColor, timeElapsed / fadeDuration);
            yield return null;
        }
    }
}
