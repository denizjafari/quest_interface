using System.Collections;
using UnityEngine;

public class VRApple : MonoBehaviour
{
    
    [Tooltip("How long in seconds the apple can exist.")]
    [SerializeField] float decayTime = 8f;

    [Header("Debug")]
    [SerializeField] bool isVerbose = false;

    // Configs
    private float existing_timer = 0f;
    private bool isHeld = false;
    private GameObject cursor;
    private GameObject basket;

    // Component References
    private Vector3 boxCenter;

    // Start is called before the first frame update
    void Start()
    {
        boxCenter = transform.position;
        basket = GameObject.Find("Basket");

        // Load game difficulty config
        GameInitConfig config = Helper.LoadGameInitConfig();

        decayTime = ControllerListener.Instance.selfDestroyTime != 0 ?
            ControllerListener.Instance.selfDestroyTime :
            decayTime;

        Invoke(nameof(DespawnApple), decayTime);
    }

    // Update is called once per frame
    void Update()
    {

        // Follow the cursor if player grabbed the apple
        if (isHeld)
        {
            transform.position = cursor.transform.position;
        }
    }

    void FixedUpdate()
    {
        // Let the object fall into the basket.
        if (isHeld && IsTargetOverBasket())
        {
            isHeld = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }


    private void OnDisable()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddMiss(); // Decrement user score.
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Basket")
        {
            ScoreManager.Instance.AddScore();
            Destroy(gameObject);
        }
    }


    void DespawnApple()
    {
        if (isVerbose) Debug.Log("Apple Timed Out");
        Destroy(gameObject);
    }


    /// <summary>
    /// Check and return a bool if this object's position is hovering over the basket.
    /// </summary>
    /// <returns>Boolean representing if this object is over a basket.</returns>
    bool IsTargetOverBasket()
    {
        Collider collider = GetComponent<Collider>(), basketCollider = basket.GetComponent<Collider>();
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
        Color initalColor = new(finalColor.r, finalColor.g, finalColor.b, 0f);

        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            renderer.material.color = Color.Lerp(initalColor, finalColor, timeElapsed / fadeDuration);
            yield return null;
        }
    }
}
