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
    [SerializeField] private bool test = false;
    private int laststate = 0;
    private Vector3 pos1;
    private Vector3 pos2;
    public float speed = 1.0f;

    private void move(int value)
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
    private void Awake()
    {
        initialX = transform.position.x;
        Debug.Log(initialX);
        initialY = transform.position.y;
        Debug.Log(initialY);
        initialZ = transform.position.z;
        Debug.Log(initialZ);

        pos1 = new Vector3(initialX, initialY, initialZ);
        pos2 = new Vector3(initialX, initialY - down, initialZ);


    }
    // Start is called before the first frame update
    void Start()
    {

    }


    void Update()
    {
        if (test)
        {
            switch (Number)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha1))
                    {
                        move(0);
                    }
                    break;
                case 1:
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha2))
                    {
                        move(0);
                    }
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha3))
                    {
                        move(0);
                    }
                    break;
                case 3:
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha4))
                    {
                        move(0);
                    }
                    break;
                case 4:
                    if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha5))
                    {
                        move(0);
                    }
                    break;
                case 5:
                    if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha6))
                    {
                        move(0);
                    }
                    break;
                case 6:
                    if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha7))
                    {
                        move(0);
                    }
                    break;
                case 7:
                    if (Input.GetKeyDown(KeyCode.Alpha8))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha8))
                    {
                        move(0);
                    }
                    break;
                case 8:
                    if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        move(1);
                    }
                    if (Input.GetKeyUp(KeyCode.Alpha9))
                    {
                        move(0);
                    }
                    break;
            }
        }
        else
        {
            switch (Number)
            {
                case 0:
                    move(UDPManager.Instance.key);
                    break;
                case 1:
                    move(UDPManager.Instance.key1);
                    break;
                case 2:
                    move(UDPManager.Instance.key2);
                    break;
                case 3:
                    move(UDPManager.Instance.key3);
                    break;
                case 4:
                    move(UDPManager.Instance.key4);
                    break;
                case 5:
                    move(UDPManager.Instance.key5);
                    break;
                case 6:
                    move(UDPManager.Instance.key6);
                    break;
                case 7:
                    move(UDPManager.Instance.key7);
                    break;
                case 8:
                    move(UDPManager.Instance.key8);
                    break;
            }
        }
    }
}


