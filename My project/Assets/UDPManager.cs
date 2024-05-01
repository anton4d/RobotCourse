using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UDPManager : MonoBehaviour
{
    // Static variable that holds the instance
    public static UDPManager Instance { get; private set; }

    // UDP Settings
    [Header("UDP Settings")]
    [SerializeField] private int UDPPort = 50195;
    [SerializeField] private bool displayUDPMessages = false;
    private UdpClient udpClient;
    private IPEndPoint endPoint;

    // ESP32 Sensor
    public int potentiometerValue { get; private set; } = 0; 
    public int potentiometerValue1 { get; private set; } = 0; 
    public int servomotor { get; private set; } = 0; 
    public int key { get; private set; } = 0;
    public int key1 { get; private set; } = 0;
    public int key2 { get; private set; } = 0;
    public int key3 { get; private set; } = 0;
    public int key4 { get; private set; } = 0;
    public int key5 { get; private set; } = 0;
    public int key6 { get; private set; } = 0;
    public int key7 { get; private set; } = 0;
    public int key8 { get; private set; } = 0; 


    void Awake()
    {
        // Assign the instance to this instance, if it is the first one
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get IP Address
        DisplayIPAddress();

        // UDP begin
        endPoint = new IPEndPoint(IPAddress.Any, UDPPort);
        udpClient = new UdpClient(endPoint);
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SendUDPMessage("LED|1", "10.126.128.155", 3002);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SendUDPMessage("LED|0", "10.126.128.155", 3002);
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
        string receivedData = Encoding.UTF8.GetString(receivedBytes);

        // Log UDP message
        if (displayUDPMessages)
        {
            Debug.Log("Received data from " + endPoint.Address.ToString() + ": " + receivedData);
        }

        // Splitting the receivedData string by the '|' character
        string[] parts = receivedData.Split('|');

        if (parts.Length == 2) 
        {
            string sensorID = parts[0];
            int value;
            if (int.TryParse(parts[1], out value))
            {
                Debug.Log("UDP message recived: " + sensorID + " "+ value);
                switch(sensorID){
                    case "potentiometer":
                    potentiometerValue = value;
                    break;
                    case "potentiometer1":
                    potentiometerValue1 = value;
                    break;
                    case "key0":
                    key = value;
                    break;
                    case "key1":
                    key1 = value;
                    break;
                    case "key2":
                    key2 = value;
                    break;
                    case "key3":
                    key3 = value;
                    break;
                    case "key4":
                    key4 = value;
                    break;
                    case "key5":
                    key5 = value;
                    break;
                    case "key6":
                    key6 = value;
                    break;
                    case "key7":
                    key7 = value;
                    break;
                    case "key8":
                    key8 = value;
                    break;
                    case "Servomotor":
                    servomotor = value;
                    break;
                }
                
            }
            else
            {
                Debug.LogError("Failed to parse the value as an integer.");
            }
        }
        else
        {
            Debug.LogError("Received data is not in the expected format.");
        }

        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // Function to send UDP message
    public void SendUDPMessage(string message, string ipAddress, int port)
    {
        UdpClient client = new UdpClient();
        try
        {
            // Convert the message string to bytes
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Send the UDP message
            client.Send(data, data.Length, ipAddress, port);
            Debug.Log("UDP message sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending UDP message: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

    void DisplayIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Debug.Log(ip.ToString());
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error fetching local IP address: " + ex.Message);
        }
    }
}
