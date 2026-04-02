using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the calibration for the scene.
/// Starts with initialPosition, then Y, then X calibration.
/// </summary>
public class CalibrationManager : MonoBehaviour
{
    #region --- Variables ---
    [Header("Debug")]
    [SerializeField] bool isVerbose = false;

    [Header("Required Components")]
    [SerializeField] GameObject rightHandVisual;
    [SerializeField] GameObject leftHandVisual;
    [SerializeField] Timer timer;
    [SerializeField] TMP_Text checklist_tmp;
    [SerializeField] TMP_Text helper_tmp;
    [SerializeField] GameObject minAppleX;
    [SerializeField] GameObject maxAppleX;

    [Header("Configs")]
    [SerializeField] Handedness currentHand = Handedness.RightHand; // Make this matter.
    [SerializeField] float countdown = 10f;

    // Private Variables
    private CalibrationState state;
    private float minX = 0f;
    private float maxX = 0f;
    private float currentY = 0f;
    private float currentZ = 0f;
    private float remainingTime;

    // Getters
    public float MinX => minX;
    public float MaxX => maxX;
    public float CurrentY => currentY;
    public float CurrentZ => currentZ;
    public float RemainingTime => remainingTime;


    private enum CalibrationState {
        None = -1,
        XCalibration = 0,
        YZCalibration = 1,
        SaveConfig = 2,
        NextScene = 3,
    }

    private enum Handedness
    {
        LeftHand,
        RightHand,
    }
    #endregion

    #region --- Unity Lifecycle ---
    private void Start()
    {
        if (rightHandVisual == null) Debug.LogError("[CalibrationManager] A RightHandVisual is required.");
        if (timer == null) Debug.LogError("[CalibrationManager] A timer is required.");

        // Set up the clock
        remainingTime = countdown;
        timer.MaximumSeconds = remainingTime;
        timer.ResetTimer();

        state = CalibrationState.YZCalibration;
        remainingTime = countdown;
    }

    private void Update()
    {
        CheckCalibration();
    }
    #endregion

    #region --- Private Methods ---
    private void CheckCalibration()
    {
        switch (state)
        {
            case CalibrationState.SaveConfig:
                // Prepare to exit the calibration
                SaveCalibrationData();
                state = CalibrationState.NextScene;
                break;
            case CalibrationState.XCalibration:
                UpdateXCalibration();
                break;
            case CalibrationState.YZCalibration:
                UpdateYZCalibration();
                break;
            case CalibrationState.NextScene:
                LoadNextScene();
                break;
        }
    }

    private void UpdateXCalibration()
    {
        if (rightHandVisual == null) return;

        Transform rh_transform = rightHandVisual.transform;
        if (remainingTime <= 0)
        {
            if (isVerbose) Debug.Log($"[CalibrationManager] X Calibration completed. Ending calibration.");
            state = CalibrationState.SaveConfig;
            checklist_tmp.text = "Checklist:\n<s>1. Vertical\n 2. Horizontal</s>";
        }

        if (minX > rh_transform.position.x &&
            Math.Abs(minX - rh_transform.position.x) > 0.05f)
        {
            minX = rh_transform.position.x;
            remainingTime = countdown;
            if (timer != null) timer.ResetTimer();

            if (isVerbose) Debug.Log($"[CalibrationManager] Updated min X: {minX}");
        }
        else if (maxX < rh_transform.position.x &&
            Math.Abs(maxX - rh_transform.position.x) > 0.05f)
        {
            maxX = rh_transform.position.x;
            remainingTime = countdown;
            if (timer != null) timer.ResetTimer();

            if (isVerbose) Debug.Log($"[CalibrationManager] Updated max X: {maxX}");
        }
        else
        {
            remainingTime -= Time.deltaTime;
        }

        UpdateMinMaxHelpers();
    }

    /// <summary>
    /// Calibrates the Y/Z position for the game by asking the user to hold their right-hand
    /// steady for 3 seconds.
    /// </summary>
    private void UpdateYZCalibration()
    {
        if (rightHandVisual == null) return;

        // Keep the right hand steady for 3 seconds.
        Vector3 rh_position = rightHandVisual.transform.position;
        if (remainingTime <= 0)
        {
            // Move to the X positional calibration.
            if (isVerbose) Debug.Log($"[CalibrationManager] Y Calibration complete. Moving to X Calibration");
            state = CalibrationState.XCalibration;

            // Reset Countdown
            remainingTime = countdown;
            timer.MaximumSeconds = remainingTime;
            if (timer != null) timer.ResetTimer();
            
            // Update Text
            checklist_tmp.text = "Checklist:\n<s>1. Vertical</s>\n 2. Horizontal";
            helper_tmp.text = "Swing your hand left and right as far as possible.";
            Debug.Log($"[CalibrationManager] State: {state}");
        }
        if (Math.Abs(rh_position.y - currentY) >= 0.08f ||
            Math.Abs(rh_position.z - currentZ) >= 0.08f)
        {
            if (isVerbose) Debug.Log($"[CalibrationManager] Y/Z position updated. Y: {rh_position.y}, Z: {rh_position.z}\nResetting countdown to {countdown}");
            remainingTime = countdown;

            currentY = rh_position.y;
            currentZ = rh_position.z;


            remainingTime = countdown;
            if (timer != null) timer.ResetTimer();
        }
        else
        {
            remainingTime -= Time.deltaTime;
        }
    }

    private void SaveCalibrationData()
    {
        if (isVerbose) Debug.Log("[CalibrationManager] Saving calibration data.");
        Helper.WritePositionalData(minX, maxX, currentY, currentZ);
    }

    private void LoadNextScene()
    {
        if (isVerbose) Debug.Log("[CalibrationManager] Moving to next scene.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void UpdateMinMaxHelpers()
    {
        minAppleX.transform.position = new Vector3(minX, currentY, currentZ);
        maxAppleX.transform.position = new Vector3(maxX, currentY, currentZ);
    }
    
    #endregion
}
