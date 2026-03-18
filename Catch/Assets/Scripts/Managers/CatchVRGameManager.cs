using Oculus.Interaction.HandGrab;
using System;
using UnityEngine;

public class CatchVRGameManager : Singleton<CatchVRGameManager>
{
    #region ----- Class Variables -----
    [Header("Required Components")]
    [SerializeField] GameObject rightHandVisual;
    [SerializeField] GameObject leftHandVisual;

    [Header("Game Configs")]
    [Tooltip("Seconds in between each fruit spawn.")]
    [SerializeField][Range(0.0f, 12.0f)] float spawnInterval = 8f;

    [Tooltip("How long a fruit can remain before despawning.")]
    [SerializeField][Range(0.0f, 12.0f)] float fruitLifespan = 8f;

    [Header("Debugging")]
    [SerializeField] bool isVerbose = false;

    // --- JSON Variables ---
    private float scalingFactor;
    private bool isBoundsOnly;
    private float ROMScale;
    private float maxArmRotation;
    private float minArmRotation;

    private float maxArmXPosition = 0.3f; // Right Max
    private float minArmXPosition = -0.3f; // Right Min
    private float armYPosition = 1.4f;
    #endregion

    #region ----- Public Getters -----
    public float ScalingFactor => scalingFactor;
    public bool IsBoundsOnly => isBoundsOnly;
    public float FruitLifespan => fruitLifespan;
    public float SpawnInterval => spawnInterval;

    // TEMP
    public float MaxArmXPosition => maxArmXPosition;
    public float MinArmXPosition => minArmXPosition;
    public bool IsOutOfBounds => IsHandOutOfBounds();

    public Transform LeftHandPosition => leftHandVisual.transform;
    public Transform RightHandPosition => rightHandVisual.transform;
    #endregion

    #region ----- Events -----
    public static event Action<HandGrabInteractable> OnSpawnApple;
    public static event Action<bool> OnScoreUpdate;
    public static event Action OnHoveringBasket;
    #endregion

    #region ----- Unity Lifecycle Functions -----
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();
        LoadGameConfigJSON();
    }

    private void Update()
    {
        CheckMinMaxPosition();
    }

    private void OnApplicationPause(bool pause)
    {
        // TODO: Add player Re-centering Logic
    }
    #endregion

    #region ----- Private Methods ------
    void LoadGameConfigJSON()
    {
        if (isVerbose) { Debug.Log("[CatchVRGameManager] Loading Configs..."); }
        GameInitConfig config = Helper.LoadGameInitConfig();
        scalingFactor = config.scaling_factor != 0 ? config.scaling_factor : ControllerListener.Instance.scaling_factor;
        isBoundsOnly = config.boundOnly;
        ROMScale = config.ROMScale;
        spawnInterval = config.spawnLag != 0 ? config.spawnLag : spawnInterval;
        //fruitLifespan = ControllerListener.Instance.selfDestroyTime ?? fruitLifespan;

        // TODO: Fix this shoulder rotation calculation for VR.
        ROMData rotationConfig = Helper.LoadShoulderRotationROM();
        if (rotationConfig != null)
        {
            maxArmRotation = rotationConfig.max / 90f * Camera.main.orthographicSize * Camera.main.aspect * ROMScale;
            minArmRotation = rotationConfig.min / 90f * Camera.main.orthographicSize * Camera.main.aspect * ROMScale;
        }
        else
        {
            maxArmRotation = 1f * Camera.main.orthographicSize * Camera.main.aspect * ROMScale;
            minArmRotation = -1f * Camera.main.orthographicSize * Camera.main.aspect * ROMScale;
        }
    }

    /// <summary>
    /// Check the current min/max position and update if there is a new min/max
    /// </summary>
    void CheckMinMaxPosition()
    {
        if (rightHandVisual == null) return;

        if (minArmXPosition > RightHandPosition.position.x &&
            Math.Abs(minArmXPosition - RightHandPosition.position.x) > 0.05f)
        {
            minArmXPosition = RightHandPosition.position.x;
            if (isVerbose) Debug.Log($"[CatchVRGameManager] Updated min X: {minArmXPosition}");
        }
        if (maxArmXPosition < RightHandPosition.position.x &&
            Math.Abs(maxArmXPosition - RightHandPosition.position.x) > 0.05f)
        { 
            maxArmXPosition = RightHandPosition.position.x;
            if (isVerbose) Debug.Log($"[CatchVRGameManager] Updated max X: {maxArmXPosition}");
        }
    }


    /// <summary>
    /// Checks if main hand is out of bounds.
    /// </summary>
    /// <returns>Boolean representing whether hand is out of bounds.</returns>
    private bool IsHandOutOfBounds()
    {
        if (Math.Abs(RightHandPosition.position.y - armYPosition) > 0.3f) return true;

        // TODO: Add a horizontal boundary check.

        return false;
    }
    #endregion

    #region ----- Public Handlers -----
    /// <summary>
    /// Handles the scoring for the game.
    /// </summary>
    /// <param name="isSuccessful">Is the score considered successful or a failure.</param>
    public static void HandleScoreUpdate(bool isSuccessful)
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("[CatchVRGameManager] ScoreManager Instance not found! Make sure it exists somewhere in the scene!");
            return;
        }

        if (isSuccessful) ScoreManager.Instance.AddScore();
        else ScoreManager.Instance.AddMiss();

        OnScoreUpdate?.Invoke(isSuccessful);
    }

    /// <summary>
    /// Notify all subscribers to OnSpawnApple that the Apple has been spawned.
    /// </summary>
    /// <param name="interactable">A HandGrabInteractable reference</param>
    public static void HandleSpawnApple(HandGrabInteractable interactable)
    {
        OnSpawnApple?.Invoke(interactable);
    }


    /// <summary>
    /// Notify all subscribers to OnHoveringBasket that the fruit object is now
    /// over the basket.
    /// </summary>
    public static void HandleBasketHover()
    {
        OnHoveringBasket?.Invoke();
    }
    #endregion
}
