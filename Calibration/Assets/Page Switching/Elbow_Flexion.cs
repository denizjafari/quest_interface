using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elbow_Flexion : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadSceneAsync("Wrist Pronation");
    }
}
