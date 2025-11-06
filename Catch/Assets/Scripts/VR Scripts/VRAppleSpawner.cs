using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Spawns Falling Apples in a 3D VR environment.
/// </summary>
public class VRAppleSpawner : MonoBehaviour
{
    // Internal Variables
    private float scaling_factor;
    private bool boundOnly;
    private float ROMScale;
    private int counter;                // Used to determine which side to spawn next apple.
    private float maxArmRotation;
    private float minArmRotation;
    private bool isSpawning = false;

    // Component References
    private GameObject currentApple;                                // To keep track of the existing apple.
    private Vector3 nextTargetPosition = new Vector3(0, 1, 0.5f);   // To store the position of the next box

    [Header("Configs")]
    [Tooltip("Interval in seconds between apple spawns.")]
    [SerializeField] float spawnLag = 3f;

    [Header("Prefabs")]
    [Tooltip("The object the player needs to catch.")]
    [SerializeField] GameObject applePrefab;                        // Initialize in editor

    // Start is called before the first frame update
    void Start()
    {
        // Load game difficulty parameters
        GameInitConfig config = Helper.LoadGameInitConfig();
        scaling_factor = config.scaling_factor;
        boundOnly = config.boundOnly;
        ROMScale = config.ROMScale;
        spawnLag = config.spawnLag != 0 ? config.spawnLag : spawnLag;       // TEMPORARY FIX. MUST UPDATE JSONS

        // TODO: Determine a way to limit rotation.
        // Load the player's max/min arm rotation and screen boundaries.
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

        nextTargetPosition = ComputeNextTargetPosition(); // Pre-compute the initial position for the first box
    }

    // Update is called once per frame
    void Update()
    {
        // Update parameter based on controller input
        scaling_factor = ControllerListener.Instance.scaling_factor;

        // Prepare the next spawn location
        if (currentApple == null && !isSpawning)
        {
            isSpawning = true;
            //StartCoroutine(DelaySpawnRoutine(nextTargetPosition));
            nextTargetPosition = ComputeNextTargetPosition();
            currentApple = Instantiate(applePrefab, nextTargetPosition, Quaternion.identity);
            isSpawning = false;
        }
    }

    
    // TODO: Create a new spawning algorithm.
    /// <summary>
    /// Compute the next spawn point.
    /// </summary>
    Vector3 ComputeNextTargetPosition()
    {
        //Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float x;

        // TODO: Implement Bound Only logic after min/max arm rotation is properly established.
        //if (boundOnly)
        //{
        //    counter++;

        //    if (counter % 2 == 1)
        //    {
        //        x = -1f;
        //    }
        //    else
        //    {
        //        x = 1f;
        //    }
        //}

        //counter++;

        //if (counter % 2 == 1)
        //{
        //    x = -1f;
        //}
        //else
        //{
        //    x = 1f;
        //}

        return new Vector3(0f, 3f, 0.5f);
    }


    /// <summary>
    /// Spawns the next apple, slowly fading it into existence, then has it fall.
    /// </summary>
    /// <param name="position">Position to spawn the next apple</param>
    IEnumerator DelaySpawnRoutine(Vector3 position)
    {
        currentApple = Instantiate(applePrefab, position, Quaternion.identity);
        currentApple.transform.localScale = new Vector3(0.4f * scaling_factor, 0.4f * scaling_factor, 1);
        currentApple.GetComponent<Rigidbody>().useGravity = false;

        StartCoroutine(currentApple.GetComponent<FallingTarget>().FadeIn(spawnLag));
        yield return new WaitForSeconds(spawnLag);

        currentApple.GetComponent<Rigidbody>().useGravity = true;
        isSpawning = false;
    }
}
