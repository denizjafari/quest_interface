using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] Image radialCircle;
    [SerializeField] float maxSeconds = 300f;
    [SerializeField] bool isCheckingOutOfBounds = true;

    // Public Variables
    public float MaximumSeconds
    {
        get { return maxSeconds; }
        set { maxSeconds = value; }
    }

    // Private Variables
    private float remainingTime;

    #region --- Unity Lifecycle ---
    void Start()
    {
        remainingTime = maxSeconds;
    }


    void Update()
    {
        HandleCountdown();
        UpdateRadialCircle();
    }
    #endregion

    #region --- Private Methods ---
    void UpdateRadialCircle()
    {
        radialCircle.fillAmount = remainingTime / maxSeconds;
    }


    void HandleCountdown()
    {
        if (remainingTime <= 0) return;

        if (isCheckingOutOfBounds && CatchVRGameManager.Instance.IsOutOfBounds)
        {
            timerText.text = $"You are out of bounds!";
            return;
        }

        // Only count down when time is above 0
        remainingTime -= Time.deltaTime;
        timerText.text = $"{(int)remainingTime / 60}:{(int)remainingTime % 60}";
    }
    #endregion

    #region --- Public Methods ---
    public void ResetTimer()
    {
        remainingTime = maxSeconds;
    }
    #endregion
}
