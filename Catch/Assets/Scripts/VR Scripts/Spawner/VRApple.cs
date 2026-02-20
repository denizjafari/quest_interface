using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer), typeof(Transform))]
public class VRApple : MonoBehaviour
{
    #region ----- Class Variables ------
    [SerializeField] AudioClip failClip;
    [SerializeField] AudioClip passClip;

    [Header("Debug")]
    [SerializeField] bool isVerbose = false;

    // State Variables
    private float appleLifespan;
    private bool isHeld = false;
    private GameObject referenceHand;

    // Component References
    private Vector3 boxCenter;
    private GameObject basket;
    #endregion

    #region ----- Unity Base Functions -----
    void Start()
    {
        // Get relevant config information
        appleLifespan = CatchVRGameManager.Instance.FruitLifespan;

        // Get Component References
        basket = GameObject.Find("Basket");
        boxCenter = transform.position;

        Invoke(nameof(TimeoutApple), appleLifespan);
    }


    private void Update()
    {
        if (isHeld && IsTargetOverBasket()) ReleaseObject();
        if (isHeld && !IsTargetOverBasket()) FollowHand();

        if (transform.position.y < -1) TimeoutApple();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Basket") DespawnApple(true);
        if (!isHeld && collision.collider.gameObject.name == "LeftHand") AttachToHand(collision.collider.gameObject);
        if (!isHeld && collision.collider.gameObject.name == "RightHand") AttachToHand(collision.collider.gameObject);

        if (isVerbose) Debug.Log($"[VRApple] Collision with {collision.gameObject.name} detected");
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
        PlaySFX(isScoring);

        CatchVRGameManager.HandleScoreUpdate(isScoring);
        Destroy(gameObject);
    }


    void PlaySFX(bool isScoring)
    {
        if (isScoring)
        {
            AudioSource.PlayClipAtPoint(passClip, gameObject.transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(failClip, gameObject.transform.position);
        }
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


    void TimeoutApple()
    {
        if (isVerbose) Debug.Log("[VRApple] Apple has timed out.");
        DespawnApple(false);
    }
    #endregion

    #region ------ MetaSDK Interactions -----
    private void ReleaseObject()
    {
        CatchVRGameManager.HandleBasketHover();
    }

    private void AttachToHand(GameObject hand)
    {
        isHeld = true;
        referenceHand = hand;
    }

    private void FollowHand()
    {
        if (referenceHand == null)
        {
            Debug.LogError("[VRApple] Reference Hand not found");
            return;
        }

        transform.position = referenceHand.transform.position + new Vector3(0, 0, 0.1f);
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
