using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;

public class PronateWrist : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    public int port = 5000; // The port to listen on
    public GameObject objectToRotate; // The game object to rotate
    public GameObject trailMarkerPrefab; // The prefab for the trail marker
    public Transform trailParent;   
    private float max = float.MinValue;
    private float min = float.MaxValue;

    void Start()
    {
        // Start the TCP listener thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    void Update()
    {
        if (connectedTcpClient != null)
        {
            // Check for incoming messages
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.DataAvailable)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                ArmData armData = JsonUtility.FromJson<ArmData>(receivedMessage);
                print(armData.wrist_pronation_right);

                RotateGameObject(armData.wrist_pronation_right);
                UpdateMinMax(armData.wrist_pronation_right);
            }
        }
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            tcpListener.Start();

            while (true)
            {
                connectedTcpClient = tcpListener.AcceptTcpClient();
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException: " + socketException.ToString());
        }
    }

    private void RotateGameObject(float rotationDegrees)
    {
        // Write pronation is very not accurate, so we won't use hard coded condition here
        // if (rotationDegrees > 90 || rotationDegrees < -90)
        // {
        //     return;
        // }
        print(rotationDegrees);
        // Rotate the game object
        Vector3 currentRotation = objectToRotate.transform.localEulerAngles;
        currentRotation.z = rotationDegrees;
        objectToRotate.transform.localEulerAngles = currentRotation;

        // Create a trail marker at the current position
        CreateTrailMarker();
    }

    private void CreateTrailMarker()
    {
        // Instantiate the trail marker prefab at the current position and rotation of the object
        Vector3 trailMarkerPosition = objectToRotate.transform.position;
        Quaternion trailMarkerRotation = objectToRotate.transform.rotation;
        GameObject orangeTrail = Instantiate(trailMarkerPrefab, trailMarkerPosition, trailMarkerRotation, trailParent);
    }

    private void UpdateMinMax(float wristPronation)
    {
        if (wristPronation > max)
        {
            max = wristPronation;
        }

        if (wristPronation < min)
        {
            min = wristPronation;
        }
    }

    private void OnDisable()
    {
        string path = "../../game_config/wrist_pronation_ROM.json";
        ROMData romData = new ROMData
        {
            max = max,
            min = min
        };
        string json = JsonUtility.ToJson(romData);
        File.WriteAllText(path, json);
        
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Abort();
        }
    }

    // This function is required to close the server correctly
    private void OnDestroy()
    {
        if (tcpListener != null)
        {
            tcpListener.Stop();
            tcpListener = null;
        }
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Abort();
            tcpListenerThread = null;
        }
    }
}