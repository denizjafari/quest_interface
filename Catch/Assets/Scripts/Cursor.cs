using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.IO;

public class UnityServer : MonoBehaviour // This class is associated with the game object that's controlled by the user.
{
    // Boilerplate code for setting up the server to receive vision input.
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Camera mainCamera;
    private float ROMScale; // A game parameter that controlls how sensitive is the game object to user's input.

    void Start()
    {
        mainCamera = Camera.main; // This is camera in unity game. Not your computer camera
        StartServer(); // Let's goo, server start.

        // Load game difficulty config
        GameInitConfig config = Helper.LoadGameInitConfig();
        ROMScale = config.ROMScale;
    }

    void StartServer() // Server start
    {
        string[] args = System.Environment.GetCommandLineArgs();
        tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        tcpListener.Start();
    }

    void Update()
    {
        ROMScale = ControllerListener.Instance.ROMScale; // Update parameter based on controller input
        
        if (tcpListener != null && tcpListener.Pending()) // Boilderplate code
        {
            tcpClient = tcpListener.AcceptTcpClient();
            stream = tcpClient.GetStream();
            StartCoroutine(ReceiveData());
        }
    }

    IEnumerator ReceiveData() // More boilerplate server code
    {
        byte[] bytes = new byte[1024];
        while (true)
        {
            if (stream.CanRead)
            {
                int length = stream.Read(bytes, 0, bytes.Length);
                if (length != 0)
                {
                    var incomingData = Encoding.ASCII.GetString(bytes, 0, length);
                    ArmRotationData armData = JsonUtility.FromJson<ArmRotationData>(incomingData);

                    // Map normalized coordinates [-90, 90] to camera space, basically using camera's width to normalize player input.
                    float mappedX = armData.shoulder_rotation_right / 90f * mainCamera.orthographicSize * mainCamera.aspect;
                    float mappedY = armData.shoulder_rotation_right / 90f * mainCamera.orthographicSize;

                    // Adjust user input by sensitivity.
                    mappedX = mappedX * ROMScale;
                    mappedY = mappedY * ROMScale;

                    transform.position = new Vector3(mappedX, 0, transform.position.z); // Actually moving this game object to the position inputed by vision module.
                }
            }
            yield return null;
        }
    }

    void OnApplicationQuit() // More boilerplate code to stop the server.
    {
        if (stream != null) stream.Close();
        if (tcpClient != null) tcpClient.Close();
        if (tcpListener != null) tcpListener.Stop();
    }
}
