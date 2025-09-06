using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class Player : MonoBehaviour
{   
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    public int port = 5000; // The port to listen on

    void Start()
    {
        // Start the TCP listener thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
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

                ArmData armData = JsonUtility.FromJson<ArmData>(receivedMessage); // Modify this if input data changes.
                print(armData.shoulder_abduction_right);

                if(armData.shoulder_abduction_right > 90f){
                    armData.shoulder_abduction_right = 90f;
                }else if(armData.shoulder_abduction_right < 5f){
                    armData.shoulder_abduction_right = 5f;
                }
               
                transform.position = new Vector3(0f,(armData.shoulder_abduction_right - 45)/12, 0f);
                // Examples of accessing posture value
                // RotateGameObject(armData.shoulder_rotation);
                // UpdateMinMax(armData.shoulder_rotation);
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

    private void OnDisable()
    {
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Abort();
        }
    }
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

   
    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }


    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            GameManager.Instance.IncreaseScore();
        }

        /* if (other.gameObject.tag == "Obstacle") {
            FindObjectOfType<GameManager>().GameOver();
        } else if (other.gameObject.tag == "Scoring") {
            FindObjectOfType<GameManager>().IncreaseScore();
        } */
    }

}
