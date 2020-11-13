using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{

    public bool timedCloud;
    // Start is called before the first frame update
    void Start()
    {
        if (timedCloud == true)
             Destroy(gameObject, WeatherController.Instance.cloudTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
