using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;

public class FlexShoulder : MonoBehaviour
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

                RotateGameObject(armData.shoulder_flexion_right);
                UpdateMinMax(armData.shoulder_flexion_right);
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
        if (rotationDegrees < 0 || rotationDegrees > 90)
        {
            return;
        }
        Vector3 currentRotation = objectToRotate.transform.localEulerAngles;
        currentRotation.z = rotationDegrees - 90;
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

    private void UpdateMinMax(float shoulderFlexion)
    {
        if (shoulderFlexion > max)
        {
            max = shoulderFlexion;
        }

        if (shoulderFlexion < min)
        {
            min = shoulderFlexion;
        }
    }

    private void OnDisable()
    {
        string path = "../../game_config/shoulder_flexion_ROM.json";
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
