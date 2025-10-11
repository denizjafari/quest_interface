using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shoulder_Rotation : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadSceneAsync("Shoulder Abduction");
    }
}
