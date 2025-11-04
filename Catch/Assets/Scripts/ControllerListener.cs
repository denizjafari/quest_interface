using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// This class listens to localhost port 5001. When it receives new data, it updates class variables, 
/// such that other files can access the most recent game parameters through this class.
/// </summary>
public class ControllerListener : MonoBehaviour
{
    // Some boilerplate code for setting up the server.
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    // Public game parameters that every other game object access
    public float selfDestroyTime;
    public float warningTime;
    public float scaling_factor;
    public float ROMScale;

    // So that other files can access variables like this is a static class.
    public static ControllerListener Instance { get; private set; }


    void Start()
    {
        // Some boilerplate code to make this class static
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Start TcpServer background thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();

        // Load game difficulty config as default values.
        GameInitConfig config = Helper.LoadGameInitConfig();
        selfDestroyTime = config.selfDestroyTime;
        warningTime = config.warningTime;
        scaling_factor = config.scaling_factor;
        ROMScale = config.ROMScale;
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            // Boilerplate code for setting up the server
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5001);
            tcpListener.Start();

            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            string clientMessage = Encoding.ASCII.GetString(incomingData);

                            // Send the incoming message to a function that converts it to json data.
                            HandleJsonData(clientMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.LogError("SocketException " + socketException.ToString());
        }
    }

    private void HandleJsonData(string jsonString) // This function is responsible for taking the message this server received and converting it into json 
    {
        GameInitConfig config = JsonUtility.FromJson<GameInitConfig>(jsonString); // Convert to json

        selfDestroyTime = config.selfDestroyTime; // Store updated selfDestroyTime into class variable
        warningTime = config.warningTime; // Store updated warningTime into class variable
        scaling_factor = config.scaling_factor; // Store updated scaling_factor into class variable
        ROMScale = config.ROMScale; // Store updated ROMScale into class variable
    }

    private void OnDestroy() // More boilerplate code to stop the server
    {
        // Stop listening thread
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Interrupt();
            tcpListenerThread.Abort();
        }

        // Stop TcpListener.
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
    }
}
