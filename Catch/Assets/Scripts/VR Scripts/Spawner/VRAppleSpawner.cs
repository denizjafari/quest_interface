using Oculus.Interaction.HandGrab;
using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns Falling Apples in a 3D VR environment.
/// </summary>
public class VRAppleSpawner : Singleton<VRAppleSpawner>
{
    #region Class Variables
    // Internal Variables
    private int counter;                // Used to determine which side to spawn next apple.
    private bool isSpawning = false;

    // Component References
    private GameObject currentApple;       // To keep track of the existing apple.
    private Vector3 nextTargetPosition;    // To store the position of the next box

    [Header("Configs")]
    [Tooltip("Set by CatchVRGameManager. Defaults to this value otherwise.")]
    [SerializeField] float spawnLag = 3f;
    [SerializeField] bool isVerbose = false;

    [Header("Prefabs")]
    [Tooltip("The object the player needs to catch.")]
    [SerializeField] GameObject applePrefab;            // Initialize in editor

    [Tooltip("Should be the spawnpoint in the tree objects.")]
    [SerializeField] Transform spawnTransformLeft;      // Position to spawn the left position target
    [SerializeField] Transform spawnTransformRight;     // Position to spawn the right position target
    #endregion

    #region Unity Base Functions
    // Start is called before the first frame update
    void Start()
    {
        if (CatchVRGameManager.Instance != null)
        {
            spawnLag = CatchVRGameManager.Instance.SpawnInterval;

            spawnTransformLeft.position = new Vector3(CatchVRGameManager.Instance.MinArmXPosition, spawnTransformLeft.position.y, CatchVRGameManager.Instance.MaxArmZPosition);
            spawnTransformRight.position = new Vector3(CatchVRGameManager.Instance.MaxArmXPosition, spawnTransformLeft.position.y, CatchVRGameManager.Instance.MaxArmZPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawning && currentApple == null)
        {
            isSpawning = true;
            SpawnApple();
        }
    }
    #endregion


    #region Methods
    void SpawnApple()
    {
        if (isVerbose) Debug.Log("[VRAppleSpawner] Spawning Apple");

        nextTargetPosition = ComputeNextTargetPosition();
        currentApple = Instantiate(applePrefab, nextTargetPosition, Quaternion.identity);
        isSpawning = false;

        //StartCoroutine(DelaySpawnRoutine(currentApple));
        CatchVRGameManager.HandleSpawnApple(currentApple.GetComponent<HandGrabInteractable>());
    }
    
    
    /// <summary>
    /// Compute the next spawn point.
    /// </summary>
    Vector3 ComputeNextTargetPosition()
    {
        Vector3 targetPosition;
        if (counter % 2 == 1)
        {
            float minArmXPosition = CatchVRGameManager.Instance.MinArmXPosition;
            spawnTransformLeft.position = new(minArmXPosition, spawnTransformLeft.position.y, spawnTransformLeft.position.z);
            targetPosition = spawnTransformLeft.position;
        }
        else
        {
            float maxArmXPosition = CatchVRGameManager.Instance.MaxArmXPosition;
            spawnTransformRight.position = new(maxArmXPosition, spawnTransformRight.position.y, spawnTransformRight.position.z);
            targetPosition = spawnTransformRight.position;
        }

        if (isVerbose) Debug.Log($"[VRAppleSpawner] Target X location: {targetPosition.x}");
        counter++;
        return targetPosition;
    }


    /// <summary>
    /// Spawns the next apple, slowly fading it into existence, then has it fall.
    /// </summary>
    /// <param name="position">Position to spawn the next apple</param>
    IEnumerator DelaySpawnRoutine(GameObject apple)
    {
        currentApple.GetComponent<Rigidbody>().useGravity = false;
        StartCoroutine(currentApple.GetComponent<VRApple>().FadeIn(spawnLag));

        yield return new WaitForSeconds(spawnLag);

        currentApple.GetComponent<Rigidbody>().useGravity = true;
    }
    #endregion
}
