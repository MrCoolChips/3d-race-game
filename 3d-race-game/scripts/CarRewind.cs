using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarRewind : MonoBehaviour
{
    private bool isRewinding = false;
    private List<CarState> carStates;
    private Rigidbody rb;
    private float recordTime = 5f;
    private int rights = 3;
    private GameObject effect;
    private KeyCode rewindKey = KeyCode.R;

    void Start()
    {
        if (PlayerPrefs.HasKey("RewindKey"))
            rewindKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RewindKey"));
        effect = GameObject.FindWithTag("Rewind");
        effect.SetActive(false);
        carStates = new List<CarState>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(rewindKey) && rights > 0)
        {
            rights--;
            StartRewind();
        }
        else if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
        }
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    void Record()
    {
        if (carStates.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            carStates.RemoveAt(carStates.Count - 1);
        }

        carStates.Insert(0, new CarState(
            transform.position,
            transform.rotation,
            rb.velocity,
            rb.angularVelocity
        ));
    }

    void Rewind()
{
    if (carStates.Count > 1) 
    {
        CarState state = carStates[0];
        transform.position = state.position;
        transform.rotation = state.rotation;

        carStates.RemoveAt(0);
    }
    else
    {
        StopRewind();
    }
}


    void StartRewind()
    {
        effect.SetActive(true);
        GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>().text = "You have " + rights + " rewinds left";
        gameObject.GetComponent<CarUserControl>().enabled = false;
        isRewinding = true;
        rb.isKinematic = true;
    }

    void StopRewind()
    {
        effect.SetActive(false);
        isRewinding = false;
        rb.isKinematic = false;

        if (carStates.Count > 0)
        {
            CarState lastState = carStates[0];
            rb.velocity = lastState.velocity;
            rb.angularVelocity = lastState.angularVelocity;
        }
        gameObject.GetComponent<CarUserControl>().enabled = true;
        GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>().text = "";
    }

    struct CarState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public CarState(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
        {
            position = pos;
            rotation = rot;
            velocity = vel;
            angularVelocity = angVel;
        }
    }
}
