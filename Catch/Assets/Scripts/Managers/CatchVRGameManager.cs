using Oculus.Interaction.HandGrab;
using System;
using UnityEngine;

public class CatchVRGameManager : Singleton<CatchVRGameManager>
{
    #region ----- Class Variables -----
    [Header("Required Components")]
    [SerializeField] GameObject rightController;

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

    private float maxArmRotationTracked;
    private float minArmRotationTracked;
    #endregion

    #region ----- Public Getters -----
    public float ScalingFactor => scalingFactor;
    public bool IsBoundsOnly => isBoundsOnly;
    public float MaxArmRotation => maxArmRotation;
    public float MinArmRotation => minArmRotation;
    public float FruitLifespan => fruitLifespan;
    public float SpawnInterval => spawnInterval;
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
