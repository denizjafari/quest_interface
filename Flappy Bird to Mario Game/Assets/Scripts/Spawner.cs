using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Pipes prefab;

    [Header("Gameplay configs")]
    [SerializeField] float spawnRate = 0.5f;
    [SerializeField] float minHeight = -3f;
    [SerializeField] float maxHeight = 5f;
    [SerializeField] float verticalGap = 3f;

    private bool isSpawningTop = true; // Determines which pipe to spawn

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    /// <summary>
    ///     Spawn the pipe obstacles with a gap between them, disabling the unused pipe on spawn.
    /// </summary>
    private void Spawn()
    {
        Pipes pipes = Instantiate(prefab, transform.position, Quaternion.identity);
        Vector3 spawnPosition = pipes.transform.position;
        float midpoint = (minHeight + maxHeight) / 2;

        // Alternate spawns to keep player alternating between width bounds
        if (isSpawningTop)
        {
            pipes.DisableBottomPipe();
            pipes.transform.position = new Vector3(spawnPosition.x, Random.Range(minHeight, midpoint), spawnPosition.z);
        }
        else
        {
            pipes.DisableTopPipe();
            pipes.transform.position = new Vector3(spawnPosition.x, Random.Range(midpoint, maxHeight), spawnPosition.z);
        }

        pipes.SetGap(verticalGap);
        isSpawningTop = !isSpawningTop;
    }

}
