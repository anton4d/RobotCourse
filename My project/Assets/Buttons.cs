using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    private float initialX;
    private float initialY;
    private float initialZ;
    [SerializeField] private int Number = 0;
    [SerializeField] private int down = 2;
    private int laststate = 0;
    private Vector3 pos1;
    private Vector3 pos2;
    public float speed = 1.0f;

    private void moveudp(int value)
    {
        if (value != laststate)
        {
            laststate = value;
            //Debug.Log(value);
            if (value == 0)
            {
                transform.position = Vector3.Lerp(pos1, pos2, 0);
            }
            if (value == 1)
            {
                transform.position = Vector3.Lerp(pos1, pos2, 1);
            }
        }

    }

    private void ButtonPress(int value)
    {
        int key = Number + 1;
        if (value == 0)
        {
            UDPManager.Instance.SendUDPMessage("Button|" + key, UDPManager.Instance.ESP32IP, UDPManager.Instance.ESP32Port);
            transform.position = Vector3.Lerp(pos1, pos2, 0);
        }
        if (value == 1)
        {
            transform.position = Vector3.Lerp(pos1, pos2, 1);
        }
    }
    private void Awake()
    {
        initialX = transform.position.x;
        initialY = transform.position.y;
        initialZ = transform.position.z;

        pos1 = new Vector3(initialX, initialY, initialZ);
        pos2 = new Vector3(initialX, initialY - down, initialZ);


    }
    // Start is called before the first frame update
    void Start()
    {

    }


    void Update()
    {

        switch (Number)
        {
            case 0:
                moveudp(UDPManager.Instance.key);
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha1))
                {
                    ButtonPress(0);
                }
                break;
            case 1:
                moveudp(UDPManager.Instance.key1);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha2))
                {
                    ButtonPress(0);
                }
                break;
            case 2:
                moveudp(UDPManager.Instance.key2);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                    ButtonPress(0);
                }
                break;
            case 3:
                moveudp(UDPManager.Instance.key3);
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    ButtonPress(0);
                }
                break;
            case 4:
                moveudp(UDPManager.Instance.key4);
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha5))
                {
                    ButtonPress(0);
                }
                break;
            case 5:
                moveudp(UDPManager.Instance.key5);
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha6))
                {
                    ButtonPress(0);
                }
                break;
            case 6:
                moveudp(UDPManager.Instance.key6);
                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha7))
                {
                    ButtonPress(0);
                }
                break;
            case 7:
                moveudp(UDPManager.Instance.key7);
                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha8))
                {
                    ButtonPress(0);
                }
                break;
            case 8:
                moveudp(UDPManager.Instance.key8);
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    ButtonPress(1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha9))
                {
                    ButtonPress(0);
                }
                break;
        }

    }
}


