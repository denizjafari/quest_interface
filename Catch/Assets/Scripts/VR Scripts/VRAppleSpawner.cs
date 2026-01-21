using System;
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
    [Tooltip("Interval in seconds between apple spawns.")]
    [SerializeField] float spawnLag = 3f;
    [SerializeField] bool isVerbose = false;

    [Header("Prefabs")]
    [Tooltip("The object the player needs to catch.")]
    [SerializeField] GameObject applePrefab;                        // Initialize in editor
    #endregion

    #region Events
    public static event Action OnSpawnApple;
    #endregion

    #region Unity Base Functions
    // Start is called before the first frame update
    void Start()
    {
        SpawnApple();
    }


    // Update is called once per frame
    void Update()
    {
        // Prepare the next spawn location
        if (!isSpawning && currentApple == null)
        {
            if (isVerbose) Debug.Log("Spawning Apple");
            isSpawning = true;
            SpawnApple();
        }
    }
    #endregion


    #region Methods
    void SpawnApple()
    {
        nextTargetPosition = ComputeNextTargetPosition();
        StartCoroutine(DelaySpawnRoutine(nextTargetPosition));
        isSpawning = false;

        OnSpawnApple?.Invoke();
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

        
        if (counter % 2 == 1)
        {
            x = -1f;
        }
        else
        {
            x = 1f;
        }

        counter++;
        return new Vector3(x, 2f, 0.5f);
    }


    /// <summary>
    /// Spawns the next apple, slowly fading it into existence, then has it fall.
    /// </summary>
    /// <param name="position">Position to spawn the next apple</param>
    IEnumerator DelaySpawnRoutine(Vector3 position)
    {
        currentApple = Instantiate(applePrefab, position, Quaternion.identity);
        currentApple.GetComponent<Rigidbody>().useGravity = false;

        StartCoroutine(currentApple.GetComponent<VRApple>().FadeIn(spawnLag));
        yield return new WaitForSeconds(spawnLag);

        currentApple.GetComponent<Rigidbody>().useGravity = true;
    }
    #endregion
}
