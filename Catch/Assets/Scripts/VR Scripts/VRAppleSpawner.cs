using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns Falling Apples in a 3D VR environment.
/// </summary>
public class VRAppleSpawner : MonoBehaviour
{
    private float scaling_factor;
    private bool boundOnly;
    private float ROMScale;
    private int counter;    // Used to determine which side to spawn next apple.

    // Component References
    private GameObject currentApple;      // To keep track of the existing apple.
    private Vector3 nextTargetPosition;   // To store the position of the next box

    [Header("Configs")]
    [Tooltip("Interval in seconds between apple spawns.")]
    [SerializeField] float spawnLag;

    [Header("Prefabs")]
    [Tooltip("The object the player needs to catch.")]
    [SerializeField] GameObject applePrefab;        // Initialize in editor

    // Start is called before the first frame update
    void Start()
    {
        // Load game difficulty parameters
        GameInitConfig config = Helper.LoadGameInitConfig();
        scaling_factor = config.scaling_factor;
        boundOnly = config.boundOnly;
        ROMScale = config.ROMScale;
        if (config.spawnLag != 0) spawnLag = config.spawnLag;           // TEMPORARY FIX. MUST UPDATE JSONS


        // TODO: Determine a way to limit rotation.

        
        nextTargetPosition = ComputeNextTargetPosition(); // Pre-compute the initial position for the first box
    }

    // Update is called once per frame
    void Update()
    {
        // Update parameter based on controller input
        scaling_factor = ControllerListener.Instance.scaling_factor;

        //// Prepare the next spawn location
        //if (currentBox == null && !isSpawning)
        //{
        //    isSpawning = true;
        //    StartCoroutine(DelaySpawnRoutine(nextTargetPosition));
        //    //currentBox = SpawnBox(nextTargetPosition);
        //    nextTargetPosition = ComputeNextTargetPosition();
        //}
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    /// <summary>
    ///     Compute the next spawn point.
    /// </summary>
    Vector3 ComputeNextTargetPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        return new Vector3(screenBounds.x, screenBounds.y, 0.5f);
    }
}
