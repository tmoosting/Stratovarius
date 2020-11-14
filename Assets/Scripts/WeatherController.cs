﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{


    // fix color stuck, color spawnedclouds
    public static WeatherController Instance;

    // scene xsize: -50 to 40, 90 total
    


    // total: 0 - 3.9
    // BASS: 1.1 - 1.5 
    // KICK:  1.5 - 2.1
    // SONG: 2.1 - 2.6

    // ENUMS
    public enum Threshold { Low, Mid, High}

    [Header("Ranges")]
    public float lowRangeMin;
    public float lowRangeMax;
    public float midRangeMin;
    public float midRangeMax;
    public float hiRangeMin;
    public float hiRangeMax;

    [Header("Thresholds")] // Going by multiplier 10, volume 0.5
    public float lowThreshold;
    public float midThreshold;
    public float hiThreshold;


    [Header("Colors")]
    public Color cloudDefault;
    public Color cloudDark;
    public Color cloudDarkest;

    [Header("Weather-Specific")] 
    public float cloudTimer;
    public int cloudLessener;
    public int cloudColorStick;
    public Threshold cloudSpawnThreshold;

    [Header("Lists")]
    public List<GameObject> cloudList = new List<GameObject>();

    [Header("Parents")]
    public Transform cloudParent;
    public Transform cloudSpawner;

    [Header("Prefabs")]
    public GameObject cloudPrefab;

    [Header("Sprites")]
    public Sprite cloudSprite1;
    public Sprite cloudSprite2;
    public Sprite cloudSprite3;
    public Sprite cloudSprite4;
    public Sprite cloudSprite5;


    private float highestPeak = 0;
    private float highestPeakX = 0;
    private GameObject highestCloud; 

    private void Awake()
    {
        Instance = this;
        ResetCloudList();
    
        highestCloud = cloudList[0];
    }
    // Update is called once per frame
    void Update()
    { 
        SetHighValues();
        CheckForWeatherChanges();
        ResetHighValues();
    }
    void ResetCloudList()
    {
        cloudList = new List<GameObject>();
        foreach (Transform child in cloudParent.transform)
            if (child.gameObject.name != "CloudSpawner")
                cloudList.Add(child.gameObject);
    }

    void SetHighValues()
    {
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
            if (Equalizer.Instance.GetFrequencyMap()[xValue] > highestPeak)
            {
                highestPeakX = xValue;
                highestPeak = Equalizer.Instance.GetFrequencyMap()[xValue];
            }
        highestPeakX *= 22.5f;
    
        float smallestDistance = 99999f;


        foreach (GameObject cloud in cloudList)
        {
            float distance = Vector3.Distance(new Vector3(cloud.transform.localPosition.x, 0, 0), new Vector3(highestPeakX, 0, 0));
            if (distance < smallestDistance)
            {
                highestCloud = cloud;
                smallestDistance = distance;
            }
        }
    }
    void ResetHighValues()
    {
        highestPeak = 0;
        highestPeakX = 0;
    }

    void CheckForWeatherChanges()
    {
        CheckForCloudColors();
        CheckForCloudSpawn();
    }

    void CheckForCloudColors()
    {
          
     //   Debug.Log("highx: " + highestX + "   highestPeak: " + highestPeak +"   closecloud: " + closestCloud.transform.localPosition.x);
      
       
            if (highestPeak > lowThreshold)
            {
            //  Debug.Log("MAX color changed! " + closestCloud.transform.localPosition.x);
            highestCloud.GetComponent<SpriteRenderer>().color = cloudDarkest;

            int cloudIndex = cloudList.IndexOf(highestCloud);

            
            GameObject cloudLeft = cloudList[cloudIndex - 1];
            GameObject cloudRight = cloudList[cloudIndex + 1];
            cloudLeft.GetComponent<SpriteRenderer>().color = cloudDark;
            cloudRight.GetComponent<SpriteRenderer>().color = cloudDark;

            if (Random.Range(0, cloudColorStick) == 0)            
                ResetCloudColors();
            

        }

        //else
        //    ResetCloudColors();



    }
    void ResetCloudColors()
    {
        foreach (GameObject obj in cloudList)        
            obj.GetComponent<SpriteRenderer>().color = cloudDefault;        
    }
    void CheckForCloudSpawn()
    {
        if (cloudSpawnThreshold == Threshold.Low)
        {
            if (CheckRange(lowRangeMin, lowRangeMax, lowThreshold) == true)
                SpawnCloud();
        }
        else if (cloudSpawnThreshold == Threshold.Mid)
        {
            if (CheckRange(lowRangeMin, lowRangeMax, midThreshold) == true)
                SpawnCloud();
        }
        else if (cloudSpawnThreshold == Threshold.High)
        {
            if (CheckRange(lowRangeMin, lowRangeMax, hiThreshold) == true)
                SpawnCloud();
        }

    }
    void SpawnCloud()
    { 
        if (Random.Range(0, cloudLessener) == 0)
        {
            GameObject cloudObj = Instantiate(cloudPrefab);
            cloudObj.transform.SetParent(cloudSpawner);
            cloudObj.transform.localPosition = new Vector3(Random.Range(-40, 40), Random.Range(-3, 3), 0);
            cloudObj.transform.localScale *= Random.Range(0.6f, 1f);
            int randomNr = Random.Range(0, 4);
            if (randomNr == 0)
                cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite1;
            else if (randomNr == 1)
                cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite2;
            else if (randomNr == 2)
                cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite3;
            else if (randomNr == 3)
                cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite4;
            else if (randomNr == 4)
                cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite5;

            float tint = Random.Range(80f, 240f);
            cloudObj.GetComponent<SpriteRenderer>().color = new Color(tint, tint, tint, Random.Range(0.4f,1f));
        }
       
    }
    bool CheckRange(float xLow, float xHigh, float threshold)
    {
        float[] spectrum = Equalizer.Instance.GetSpectrum();
        float sumTotal = 0;
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
        {
            if (xValue >= xLow && xValue <= xHigh)
            {
                sumTotal += Equalizer.Instance.GetFrequencyMap()[xValue];
            }
        }
        if (sumTotal > threshold)
            return true;

        return false;
    }
}