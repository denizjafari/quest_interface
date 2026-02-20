
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] float maxSeconds = 300f;
    [SerializeField] string header = "Timer:";

    private float remainingTime;


    void Start()
    {
        remainingTime = maxSeconds;
    }


    void Update()
    {
        if (remainingTime <= 0) return;

        // Only count down when time is above 0
        remainingTime -= Time.deltaTime;
        timerText.text = $"{header} {(int)remainingTime / 60}:{(int)remainingTime % 60}";
    }
}
