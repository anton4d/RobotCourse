using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potentiometer : MonoBehaviour
{
    [SerializeField] private float minInputValue = 0f;
    [SerializeField] private float maxInputValue = 1023f;
    [SerializeField] private float minRotationDegree = 0f;
    [SerializeField] private float maxRotationDegree = 270f;

    private float lastvalue;
    private float initialRotationX;
    private float initialRotationY;
    private float initialRotationZ;

    //[SerializeField] private float testValue = 0f;
    //[SerializeField] private bool useTestValue = false;

    [SerializeField] private int Number = 0;

    private void Awake()
    {
        initialRotationX = transform.localEulerAngles.x;
        initialRotationY = transform.localEulerAngles.y;
        initialRotationZ = transform.localEulerAngles.z;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Number == 0)
        {
            float value = UDPManager.Instance.potentiometerValue;
            if (value != lastvalue)
            {
                lastvalue = value;
                Debug.Log("current value:"+value + " last value:"+ lastvalue);
                RotateObjecty(value);
                
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateObjecty(800);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                RotateObjecty(0);
            }


        }
        else
        {
            float value = UDPManager.Instance.potentiometerValue1;
            if (value != lastvalue)
            {
                Debug.Log("current value:"+value + " last value:"+ lastvalue);
                RotateObjectz(value);
                lastvalue = value;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateObjectz(800);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                RotateObjectz(0);
            }
        }
    }

    private void RotateObjecty(float value)
    {
        float mappedRotation = MapValue(value, minInputValue, maxInputValue, minRotationDegree, maxRotationDegree);

        Vector3 rotationVector = new Vector3(initialRotationX, mappedRotation, initialRotationZ);

        transform.localRotation = Quaternion.Euler(rotationVector);
    }

    private void RotateObjectz(float value)
    {
        float mappedRotation = MapValue(value, minInputValue, maxInputValue, minRotationDegree, maxRotationDegree);

        Vector3 rotationVector = new Vector3(initialRotationX, initialRotationY, mappedRotation);

        transform.localRotation = Quaternion.Euler(rotationVector);
    }



    private float MapValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }
}
