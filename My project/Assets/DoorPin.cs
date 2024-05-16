using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPin : MonoBehaviour
{
    private float initialX;
    private float initialY;
    private float initialZ;

    [SerializeField] private int down = 2;
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
        initialY = transform.position.y;
        initialZ = transform.position.z;

        pos1 = new Vector3(initialX, initialY, initialZ);
        pos2 = new Vector3(initialX - down, initialY, initialZ);


    }

    void Start()
    {

    }

    void Update()
    {
        if (UDPManager.Instance.servomotor == 90)
        {
            move(1);
        }
        if (UDPManager.Instance.servomotor== 0)
        {
            move(0);
        }
    }
}
