using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Helper : MonoBehaviour // This script contains helper functions to read and write to file system.
{
    public static GameInitConfig LoadGameInitConfig(){ // Loading initial game parameters set by the GUI.
        // Relative to game executable
        string path = "game_config/GameInitConfig.json"; //"../../game_config/GameInitConfig.json";

        // Read file
        string jsonString = File.ReadAllText(path);

        // Convert to json
        GameInitConfig config = JsonUtility.FromJson<GameInitConfig>(jsonString);

        return config;
    }

    public static ControllerConfig LoadControllerConfig(){ // Loading controller parameters set by the GUI.
        // Relative to game executable
        string path = "controllers/controller_config.json";

        // Read file
        string jsonString = File.ReadAllText(path);

        // Convert to json
        ControllerConfig config = JsonUtility.FromJson<ControllerConfig>(jsonString);

        return config;
    }

    public static ROMData LoadShoulderRotationROM()
    {
        string path = "game_config/shoulder_rotation_ROM.json";
        string jsonString = File.ReadAllText(path);

        ROMData rotationConfig = JsonUtility.FromJson<ROMData>(jsonString);

        return rotationConfig;
    }

    public static void SetGameResult(string result){ // Sending game results to file system so that GUI can use this to display message.
        // Relative to game executable
        string path = "game_config/game_result.json";

        File.WriteAllText(path, result);
    }
}
