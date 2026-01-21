using System.Collections;
using UnityEngine;

using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;

[RequireComponent(typeof(Collider), typeof(Renderer), typeof(Rigidbody))]
[RequireComponent(typeof(Transform))]
public class VRApple : MonoBehaviour
{
    #region ----- Class Variables ------
    [Header("Debug")]
    [SerializeField] bool isVerbose = false;

    // State Variables
    private float appleLifespan;
    private bool isHeld = false;

    // Component References
    private Vector3 boxCenter;
    private Rigidbody rb;
    private GameObject basket;
    #endregion

    #region ----- Unity Base Functions -----
    void Start()
    {
        // Get relevant config information
        appleLifespan = CatchVRGameManager.Instance.FruitLifespan;

        // Get Component References
        rb = gameObject.GetComponent<Rigidbody>();
        basket = GameObject.Find("Basket");
        boxCenter = transform.position;

        Invoke(nameof(TimeoutApple), appleLifespan);
    }


    private void Update()
    {
        CheckAppleHeldState();
    }


    void FixedUpdate()
    {
        if (isHeld && !IsTargetOverBasket())
        {
            rb.useGravity = false;
            FollowController();
        }
        else
        {
            rb.useGravity = true;
        }
    }


    private void OnDisable()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddMiss(); // Decrement user score.
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Basket") DespawnApple(true);
    }
    #endregion

    #region ------ State Management ------
    /// <summary>
    /// Prepares the apple to be despawned and determine whether it is a
    /// successful score or not.
    /// </summary>
    /// <param name="isScoring">True if success, false otherwise.</param>
    void DespawnApple(bool isScoring)
    {
        CatchVRGameManager.HandleScoreUpdate(isScoring);
        Destroy(gameObject);
    }


    /// <summary>
    /// Check and return a bool if this object's position is hovering over the basket.
    /// </summary>
    /// <returns>Boolean representing if this object is over a basket.</returns>
    bool IsTargetOverBasket()
    {
        Collider collider = GetComponent<Collider>();
        Collider basketCollider = basket.GetComponent<Collider>();
        float xCenter = collider.bounds.center.x, xMin = basketCollider.bounds.min.x, xMax = basketCollider.bounds.max.x;

        // Add a bit of buffer space for it to be easier to drop the apple into the basket
        return xCenter < xMax - 0.2 && xCenter > xMin + 0.2;
    }


    /// <summary>
    /// checks to see if the apple is considered held or not and sets isHeld accordingly.
    /// </summary>
    void CheckAppleHeldState()
    {
        if (IsTargetOverBasket())
        {
            isHeld = false;
            return;
        }
        // TODO: If the apple is overlapping with the hands, set isHeld to true;
        else if (false)
        {

        }
        else
        {
            isHeld = false;
        }
        
    }


    /// <summary>
    /// Helper function to have this gameObject follow the controller.
    /// </summary>
    void FollowController()
    {
        // TODO: Find the controller's position and update the object's position to it.
    }


    void TimeoutApple()
    {
        if (isVerbose) Debug.Log("[VRApple] Apple has timed out.");
        DespawnApple(false);
    }
    #endregion

    #region ------ Animation Functions ------
    /// <summary>
    /// Fades in the target, automatically starting the apple to be invisible.
    /// </summary>
    /// <param name="fadeDuration">The seconds to fade the target in.</param>
    /// <returns></returns>
    public IEnumerator FadeIn(float fadeDuration)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color finalColor = renderer.material.color;
        Color initalColor = new(finalColor.r, finalColor.g, finalColor.b, 0.5f);

        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            renderer.material.color = Color.Lerp(initalColor, finalColor, timeElapsed / fadeDuration);
            yield return null;
        }
    }
    #endregion
}
