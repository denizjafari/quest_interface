using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreViewer : MonoBehaviour
{
    [SerializeField] TMP_Text scoreUI;
    [SerializeField] string header = "Score:";


    #region --- Unity Lifecycle Functions ----
    private void Start()
    {
        UpdateScoreText(true);  // Initialize the score.
    }

    private void OnEnable()
    {
        CatchVRGameManager.OnScoreUpdate += UpdateScoreText;
    }

    private void OnDisable()
    {
        CatchVRGameManager.OnScoreUpdate -= UpdateScoreText;
    }
    #endregion

    void UpdateScoreText(bool isSuccessful)
    {
        if (!isSuccessful) return;

        int currentScore = ScoreManager.Instance.Score;
        scoreUI.text = $"{header} {currentScore}";
    }
}
