using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shoulder_Abduction : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadSceneAsync("Shoulder Flexion");
    }
}
