using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager>
{
    // Some boilerplate to make these 2 variables static.
    public int Score { get; private set; }
    public int Missed { get; private set; }
    public string url; // This variable contains a url that records which route to send game event information.

    protected override void Awake()
    {
        base.Awake(); // Boilerplate for static class.

        // Based on controller config, set appropriate route.
        ControllerConfig config = Helper.LoadControllerConfig();
        switch(config.mode)
        {
            case "static":
                url = "http://localhost:8000/static";
                break;
            case "logic":
                url = "http://localhost:8000/logic";
                break;
            case "advance_logic":
                url = "http://localhost:8000/advance_logic";
                break;
            case "rl":
                url = "http://localhost:8000/rl";
                break;
            default:
                Debug.LogError("Invalid mode value in controller config.");
                break;
        }
    }

    public void AddScore() // Increment user score by one
    {
        Score++;
        StartCoroutine(UpdateController(true)); // Let controller know user scored.
    }

    public void AddMiss() // Decrement user score by one
    {
        Missed++;
        StartCoroutine(UpdateController(false)); // Let controller know user missed.
    }

    IEnumerator UpdateController(bool score)
    {
        // Convert user performance data into string to send to controller server.
        UserPerformance userPerformance = new UserPerformance();
        userPerformance.score = score;
        string performanceData = JsonUtility.ToJson(userPerformance);
        
        // More boilerplate
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(performanceData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest(); // Non-blocking
        }
    }

    void OnApplicationQuit() // Writes user's gaming score into file system
    {
        // Write to exit json
        ExitJson exitJson = new ExitJson();
        exitJson.score = Score;
        exitJson.miss = Missed;
        string content = JsonUtility.ToJson(exitJson);
        
        Helper.SetGameResult(content);
    }
}
