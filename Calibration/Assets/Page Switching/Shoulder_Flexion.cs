using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shoulder_Flexion : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadSceneAsync("Elbow Flexion");
    }
}
