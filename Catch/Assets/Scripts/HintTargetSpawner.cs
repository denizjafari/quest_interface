using UnityEngine;

public class HintBoxSpawner : MonoBehaviour
{
    [Header("Prefab Parameters")]
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject hintPrefab;

    [Header("Gameplay Parameters")]
    [SerializeField] int counter = 0;

    // Config Variables
    private float scaling_factor;
    private bool boundOnly;

    // Component References
    private GameObject currentBox; // To keep track of the existing box
    private GameObject hintBox; // To display the hint for the next box
    private Vector3 nextBoxPosition; // To store the position of the next box

    private void Start()
    {
        // Load game difficulty parameters
        GameInitConfig config = Helper.LoadGameInitConfig();
        scaling_factor = config.scaling_factor;
        boundOnly = config.boundOnly;

        // Pre-compute the initial position for the first box
        nextBoxPosition = ComputeNextBoxPosition();
    }

    /// <summary>
    ///     Compute the next spawn point.
    /// </summary>
    private Vector3 ComputeNextBoxPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); // Access the camera boundary size. (Because we might be only spawning at the boundary.)
        float x, y;

        if (boundOnly)
        {
            counter++; // This counter decides which side to spawn on

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
        else // If boundOnly game parameter is false, enter below
        {
            // Basically just randomly selecting a x and y value.
            x = UnityEngine.Random.Range(-screenBounds.x + (boxPrefab.GetComponent<Renderer>().bounds.size.x / 2) * scaling_factor, screenBounds.x - (boxPrefab.GetComponent<Renderer>().bounds.size.x / 2) * scaling_factor);
            y = UnityEngine.Random.Range(-screenBounds.y + (boxPrefab.GetComponent<Renderer>().bounds.size.y / 2) * scaling_factor, screenBounds.y - (boxPrefab.GetComponent<Renderer>().bounds.size.y / 2) * scaling_factor);
        }

        // Return the next spawn point location.
        /// NOTE! Y value is calculated here but not used below, in the future if we're creating another game for vertical reach, simply swap it out.
        return new Vector3(x, 0, 0); // new Vector3(0, y, 0); Use this if you want to create vertical reach game.
    }

    /// <summary>
    ///     Create a hint object at the given location.
    /// </summary>
    private void ShowHint(Vector3 position)
    {
        hintBox = Instantiate(hintPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Creates and returns a new box object at the given location.
    /// </summary>
    /// <returns>A Box Prefab GameObject</returns>
    private GameObject SpawnBox(Vector3 position)
    {
        GameObject newBox = Instantiate(boxPrefab, position, Quaternion.identity);
        newBox.transform.localScale = new Vector3(0.4f * scaling_factor, 0.4f * scaling_factor, 1);
        return newBox;
    }

    private void Update() 
    {
        // Update parameter based on controller input
        scaling_factor = ControllerListener.Instance.scaling_factor;

        // Can be null if either user successfully reached for the box or the box naturally decays.
        if (currentBox == null) 
        {
            // Remove the existing hint box
            if (hintBox) Destroy(hintBox);

            currentBox = SpawnBox(nextBoxPosition); // Go spawn a box that the position of the previous hint object

            nextBoxPosition = ComputeNextBoxPosition();
            ShowHint(nextBoxPosition);
        }
    }
}
