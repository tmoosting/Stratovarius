using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    public static LightningController Instance;


    public GameObject lightningPrefab;
     

    void Awake()
    {
        Instance = this; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject obj = Instantiate(lightningPrefab); 

        }
    }

    public void SpawnLightning()
    {
        // Find the highest bass zone and mid zone peak    
        float highestBassPeak = 0;
        float highestMidPeak = 0;
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)        
            if (xValue > WeatherController.Instance.subBassMin && xValue < WeatherController.Instance.midBassMax)            
                if (Equalizer.Instance.GetFrequencyMap()[xValue] > highestBassPeak)                
                    highestBassPeak = Equalizer.Instance.GetFrequencyMap()[xValue];
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
            if (xValue > WeatherController.Instance.lowMidMin && xValue < WeatherController.Instance.hiMidMax)
                if (Equalizer.Instance.GetFrequencyMap()[xValue] > highestMidPeak)
                    highestMidPeak = Equalizer.Instance.GetFrequencyMap()[xValue];

        if (highestBassPeak > WeatherController.Instance.lightningThreshold && highestMidPeak > WeatherController.Instance.lightningThreshold)
        {
            if (Random.Range(0, WeatherController.Instance.lightningLessener) == 0)
            CreateBolt();
        }

    }

    void CreateBolt()
    {
        // random cloud at first
        GameObject cloud = WeatherController.Instance.cloudList[Random.Range(0, WeatherController.Instance.cloudList.Count - 1)];

        GameObject boltObj = Instantiate(lightningPrefab);
        boltObj.transform.SetParent(WeatherController.Instance.cloudParent.transform);
        LightningBolt bolt = boltObj.GetComponent<LightningBolt>();
        Vector3 startPos = new Vector3(cloud.transform.localPosition.x + WeatherController.Instance.lightningXOffset, cloud.transform.localPosition.y-WeatherController.Instance.lightningYOffset, 1);
        Vector3 endPos = new Vector3(cloud.transform.localPosition.x + WeatherController.Instance.lightningXOffset, 0, 1);
        bolt.StartPosition = startPos;
        bolt.EndPosition = endPos; 

        //  bolt.Trigger(startPos, endPos);
        //  bolt.Trigger( );
        Destroy(boltObj, WeatherController.Instance.lightningDuration); 
    }
}
