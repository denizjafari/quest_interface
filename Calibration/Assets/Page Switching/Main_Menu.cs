using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Shoulder Rotation");
    }

    public void Instruction(){
        SceneManager.LoadSceneAsync("Instruction");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor, so we use this to stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Quits the application
        Application.Quit();
        #endif
    }
}
