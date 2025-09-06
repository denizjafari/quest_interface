using UnityEngine;

public class Pipes : MonoBehaviour
{
    [Header("Pipe Positional Data")]
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;

    [Header("Gameplay Configs")]
    [SerializeField] float speed = 5f;

    private float gap = 3f;
    private float leftEdge; // Left side of the screen.

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
        top.position += Vector3.up * gap / 2;
        bottom.position += Vector3.down * gap / 2;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

    public void SetGap(float gapSize)
    {
        gap = gapSize;
    }

    public void DisableTopPipe()
    {
        top.gameObject.SetActive(false);
    }

    public void DisableBottomPipe()
    {
        bottom.gameObject.SetActive(false);
    }
}
