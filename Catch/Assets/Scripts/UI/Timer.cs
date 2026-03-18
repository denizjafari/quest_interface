using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] Image radialCircle;
    [SerializeField] float maxSeconds = 300f;

    private float remainingTime;


    void Start()
    {
        remainingTime = maxSeconds;
    }


    void Update()
    {
        HandleCountdown();
        UpdateRadialCircle();
    }

    void UpdateRadialCircle()
    {
        radialCircle.fillAmount = remainingTime / maxSeconds;
    }


    void HandleCountdown()
    {
        if (remainingTime <= 0) return;

        if (CatchVRGameManager.Instance.IsOutOfBounds)
        {
            timerText.text = $"You are out of bounds!";
            return;
        }

        // Only count down when time is above 0
        remainingTime -= Time.deltaTime;
        timerText.text = $"{(int)remainingTime / 60}:{(int)remainingTime % 60}";
    }    
}
