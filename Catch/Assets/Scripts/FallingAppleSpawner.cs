using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FallingAppleSpawner : MonoBehaviour
{
    [Header("Prefab Parameters")]
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject hintPrefab;

    [Header("Gameplay Parameters")]
    [SerializeField] int counter = 0;   // Used to determine where to spawn the next apple.
    [SerializeField] float spawnLag = 2f;

    // Config Variables
    private float scaling_factor;
    private bool boundOnly;
    private float ROMScale;
    private float maxArmRotation;
    private float minArmRotation;
    private bool isSpawning = false;

    // Component References
    private GameObject currentBox; // To keep track of the existing box
    private GameObject hintBox; // To display the hint for the next box
    private Vector3 nextTargetPosition; // To store the position of the next box

    // Start is called before the first frame update
    void Start()
    {
        // Load game difficulty parameters
        GameInitConfig config = Helper.LoadGameInitConfig();
        scaling_factor = config.scaling_factor;
        boundOnly = config.boundOnly;
        ROMScale = config.ROMScale;

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

        // Pre-compute the initial position for the first box
        nextTargetPosition = ComputeNextTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // Update parameter based on controller input
        scaling_factor = ControllerListener.Instance.scaling_factor;

        // Prepare the next spawn location
        if (currentBox == null && !isSpawning)
        {
            isSpawning = true;
            StartCoroutine(DelaySpawnRoutine(nextTargetPosition));
            //currentBox = SpawnBox(nextTargetPosition);
            nextTargetPosition = ComputeNextTargetPosition();
        }
    }

    /// <summary>
    ///     Compute the next spawn point.
    /// </summary>
    Vector3 ComputeNextTargetPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float x, y;

        if (boundOnly)
        {
            counter++;

            if (counter % 2 == 1) // If counter is an odd number, spawn on the left/bottom
            {
                x = -screenBounds.x + (boxPrefab.GetComponent<Renderer>().bounds.size.x / 2) * scaling_factor;
                y = -screenBounds.y + (boxPrefab.GetComponent<Renderer>().bounds.size.y / 2) * scaling_factor;
            }
            else // If counter is an even number, spawn on the right/top
            {
                x = screenBounds.x - (boxPrefab.GetComponent<Renderer>().bounds.size.x / 2) * scaling_factor;
                y = screenBounds.y - (boxPrefab.GetComponent<Renderer>().bounds.size.y / 2) * scaling_factor;
            }
        }
        else
        {
            counter++;

            if (counter % 2 == 1) // If counter is an odd number, spawn on the left/bottom
            {
                x = minArmRotation;
            }
            else // If counter is an even number, spawn on the right/top
            {
                x = maxArmRotation;
            }
        }

        return new Vector3(x, screenBounds.y, 0);
    }

    /// <summary>
    ///     Create a hint object at the given location.
    /// </summary>
    void SpawnHint(Vector3 position)
    {
        hintBox = Instantiate(hintPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Creates and returns a new box object at the given location.
    /// </summary>
    /// <returns>A Box Prefab GameObject</returns>
    GameObject SpawnBox(Vector3 position)
    {
        GameObject newBox = Instantiate(boxPrefab, position, Quaternion.identity);
        newBox.transform.localScale = new Vector3(0.4f * scaling_factor, 0.4f * scaling_factor, 1);
        return newBox;
    }

    IEnumerator DelaySpawnRoutine(Vector3 position)
    {
        yield return new WaitForSeconds(spawnLag);
        currentBox = Instantiate(boxPrefab, position, Quaternion.identity);
        currentBox.transform.localScale = new Vector3(0.4f * scaling_factor, 0.4f * scaling_factor, 1);
        isSpawning = false;
    }
}

