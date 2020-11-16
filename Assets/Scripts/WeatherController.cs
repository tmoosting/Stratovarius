using System.Collections;
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
    public enum Preset { HeavyBass, Pop, Classical }
    public enum Threshold { Low, Mid, High }

    [Header("Preset")]
    public Preset preset;

    [Header("Debug")]
    public int debugRange;
    public bool showHeightIndicator;
    public float heightIndicatorHeight;

    [Header("Ranges")]    
    public float subBassMin;
    public float subBassMax;
    public float midBassMin;
    public float midBassMax;
    public float lowMidMin;
    public float lowMidMax;
    public float hiMidMin;
    public float hiMidMax;

    [Header("Thresholds 1")] // Going by multiplier 10, volume 0.5
    public float colorThresholdHeavyBass;
    public float rainThresholdHeavyBass;
    public float snowThresholdHeavyBass;
    public float lightningThresholdHeavyBass;
    [Header("Thresholds 2")]
    public float colorThresholdPop;
    public float rainThresholdPop;
    public float snowThresholdPop;
    public float lightningThresholdPop;
    [Header("Thresholds 3")]
    public float colorThresholdClassical;
    public float rainThresholdClassical;
    public float snowThresholdClassical;
    public float lightningThresholdClassical;
     

    [Header("Clouds")]
 //   public float cloudRestoreRate; 
    public int cloudColorStick; // 1 in value chance of colors resetting every frame
    public int cloudNeighborAmount;
    public int cloudZoneNeighborAmount;
    public bool zoneClouds;
    public bool resetColors;
    public Threshold cloudSpawnThreshold;

    [Header("Lightning")]
    public float lightningDuration;
    public float lightningXOffset;
    public float lightningYOffset; 
    public int lightningLessener; // chance of 1 in variable to spawn 

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
    private GameObject nextHighestCloud;
    [HideInInspector]
    public List<GameObject> cloudList = new List<GameObject>();
    private List<GameObject> foundClouds;
    private List<GameObject> foundZoneClouds;

    private Dictionary<float, float> zoneMapping;
    [HideInInspector]
    public float colorThreshold;
    [HideInInspector]
    public float rainThreshold; 
    [HideInInspector]
    public float snowThreshold;
    [HideInInspector]
    public float lightningThreshold;

    private void Awake()
    {
        Instance = this;
        ResetCloudList();

        highestCloud = cloudList[0];



    }
    // Update is called once per frame
    void Update()
    {
        LoadSettings();
        FindHighValues();
        if (debugRange == 0)
            CheckForWeatherChanges();
        else
            ShowCloudZones();
        ResetHighValues();
    }
    void LoadSettings()
    {
        if (preset == Preset.HeavyBass)
        {
            colorThreshold = colorThresholdHeavyBass;
            rainThreshold = rainThresholdHeavyBass;
            snowThreshold = snowThresholdHeavyBass;
            lightningThreshold = lightningThresholdHeavyBass;
        }
        else if (preset == Preset.Pop)
        {
            colorThreshold = colorThresholdPop;
            rainThreshold = rainThresholdPop;
            snowThreshold = snowThresholdPop;
            lightningThreshold = lightningThresholdPop;
        }
        else if (preset == Preset.Classical)
        {
            colorThreshold = colorThresholdClassical;
            rainThreshold = rainThresholdClassical;
            snowThreshold = snowThresholdClassical;
            lightningThreshold = lightningThresholdClassical;
        }

        zoneMapping = new Dictionary<float, float>();
        zoneMapping.Add(subBassMin, subBassMax);
        zoneMapping.Add(midBassMin, midBassMax);
        zoneMapping.Add(lowMidMin, lowMidMax);
        zoneMapping.Add(hiMidMin, hiMidMax);
    }

    void CheckForWeatherChanges()
    {


        if (zoneClouds == true)
            FindZoneClouds();

        ChangeCloudColors();

        RainController.Instance.SpawnRain(); 

        RainController.Instance.SpawnSnow();

        LightningController.Instance.SpawnLightning();


        //    CheckForCloudSpawn();
    }
    void FindHighValues()
    {

        // Find the highest peak and its xvalue     
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
            if (Equalizer.Instance.GetFrequencyMap()[xValue] > highestPeak)
            {
                highestPeakX = xValue;
                highestPeak = Equalizer.Instance.GetFrequencyMap()[xValue];
            }
        highestPeakX *= Equalizer.Instance.stretcher;
        highestPeakX -= Equalizer.Instance.stretcher;

      


        foundClouds = new List<GameObject>();
        // find the closest cloud to that xvalue
        float smallestDistance = 99999f;
        foreach (GameObject cloud in cloudList)
        {
            float distance = Vector2.Distance(new Vector2(cloud.transform.localPosition.x, 0), new Vector2(highestPeakX, 0));
            if (distance < smallestDistance)
            {
                highestCloud = cloud;
                smallestDistance = distance;
            }
        }
        foundClouds.Add(highestCloud);

        // find next set of closest clouds
        for (int i = 0; i < cloudNeighborAmount; i++)
        {
            smallestDistance = 99999f;
            foreach (GameObject cloud in cloudList)
            {
                if (foundClouds.Contains(cloud) == false)
                {
                    float distance = Vector2.Distance(new Vector2(cloud.transform.localPosition.x, 0), new Vector2(highestPeakX, 0));
                    if (distance < smallestDistance)
                    {
                        nextHighestCloud = cloud;
                        smallestDistance = distance;
                    }
                }
            }
            foundClouds.Add(nextHighestCloud);
        }
 
        // list of clouds die iets van halfzwart worden 
    }

    void FindZoneClouds()
    {
        foundZoneClouds = new List<GameObject>();

        foreach (float zoneMin in zoneMapping.Keys)
        {
            float zoneMax = zoneMapping[zoneMin];

            // Find the highest peak and its xvalue     
            float highestZonePeak = 0;
            float highestZonePeakX = 0;
            foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
            {
                if (xValue > zoneMin && xValue < zoneMax)
                {
                    if (Equalizer.Instance.GetFrequencyMap()[xValue] > highestZonePeakX)
                    {
                        highestZonePeakX = xValue;
                        highestZonePeak = Equalizer.Instance.GetFrequencyMap()[xValue];
                    }
                }
            }

            if (highestZonePeak > colorThreshold )
            {
                highestZonePeakX *= Equalizer.Instance.stretcher;
                //    highestZonePeakX -= Equalizer.Instance.stretcher;



                GameObject highestZoneCloud = null;
                GameObject nextHighestZoneCloud = null;
                // find clouds closest to highest cloud in zone 
                float smallestDistance = 99999f;
                foreach (GameObject cloud in cloudList)
                {
                    float distance = Vector2.Distance(new Vector2(cloud.transform.localPosition.x, 0), new Vector2(highestZonePeakX, 0));
                    if (distance < smallestDistance)
                    {
                        highestZoneCloud = cloud;
                        smallestDistance = distance;
                    }
                }
                foundZoneClouds.Add(highestZoneCloud);

                // find next set of closest clouds
                for (int i = 0; i < cloudZoneNeighborAmount; i++)
                {
                    smallestDistance = 99999f;
                    foreach (GameObject cloud in cloudList)
                    {
                        if (foundZoneClouds.Contains(cloud) == false)
                        {
                            float distance = Vector2.Distance(new Vector2(cloud.transform.localPosition.x, 0), new Vector2(highestZonePeakX, 0));
                            if (distance < smallestDistance)
                            {
                                nextHighestZoneCloud = cloud;
                                smallestDistance = distance;
                            }
                        }
                    }
                    foundZoneClouds.Add(nextHighestZoneCloud);
                }
            }
          
          //  Debug.Log("  x map " + zoneMin+ "  size of list" + foundZoneClouds.Count);

        }


    }


    void ResetHighValues()
    {
        highestPeak = 0;
        highestPeakX = 0;
    }



    void ChangeCloudColors()
    {
        //   Debug.Log(" amount " + foundClouds.Count);
        //   Debug.Log("highx: " + highestX + "   highestPeak: " + highestPeak +"   closecloud: " + closestCloud.transform.localPosition.x);
        if (highestPeak > colorThreshold)
        {
            if (zoneClouds)
            {
                foreach (GameObject cloudObj in foundZoneClouds)
                {
                    if (foundClouds.Contains(cloudObj) == false)
                    {
                        Cloud cloud = cloudObj.GetComponent<Cloud>();
                        cloud.ActivateSecondary(foundZoneClouds.IndexOf(cloudObj));
                    }
                }
            }
   
            foreach (GameObject cloudObj in foundClouds)
            {
                Cloud cloud = cloudObj.GetComponent<Cloud>();
                cloud.ActivateCloud(foundClouds.IndexOf(cloudObj));
            } 
          
                    ResetCloudColors(); 


        }
    
    }
    void ResetCloudColors()
    {
        if (resetColors == true)
        {
            foreach (GameObject obj in cloudList)
            {
                if (Random.Range(0, cloudColorStick) == 0)
                    obj.GetComponent<Cloud>().ResetCloudColor();

            }
        }

    }
    void CheckForCloudSpawn()
    {
        //if (cloudSpawnThreshold == Threshold.Low)
        //{
        //    if (CheckRange(subBassMin, subBassMax, lowThreshold) == true)
        //        SpawnCloud();
        //}
        //else if (cloudSpawnThreshold == Threshold.Mid)
        //{
        //    if (CheckRange(subBassMin, subBassMax, midThreshold) == true)
        //        SpawnCloud();
        //}


    }
    void SpawnCloud()
    {
        //if (Random.Range(0, cloudLessener) == 0)
        //{
        //    GameObject cloudObj = Instantiate(cloudPrefab);
        //    cloudObj.transform.SetParent(cloudSpawner);
        //    cloudObj.transform.localPosition = new Vector3(Random.Range(-40, 40), Random.Range(-3, 3), 0);
        //    cloudObj.transform.localScale *= Random.Range(0.6f, 1f);
        //    int randomNr = Random.Range(0, 4);
        //    if (randomNr == 0)
        //        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite1;
        //    else if (randomNr == 1)
        //        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite2;
        //    else if (randomNr == 2)
        //        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite3;
        //    else if (randomNr == 3)
        //        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite4;
        //    else if (randomNr == 4)
        //        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudSprite5;

        //    float tint = Random.Range(80f, 240f);
        //    cloudObj.GetComponent<SpriteRenderer>().color = new Color(tint, tint, tint, Random.Range(0.4f, 1f));
        //}

    }
    bool CheckRange(float xLow, float xHigh, float threshold)
    {
        //float[] spectrum = Equalizer.Instance.GetSpectrum();
        //float sumTotal = 0;
        //foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
        //{
        //    if (xValue >= xLow && xValue <= xHigh)
        //    {
        //        sumTotal += Equalizer.Instance.GetFrequencyMap()[xValue];
        //    }
        //}
        //if (sumTotal > threshold)
        //    return true;

        return false;
    }
    void ShowCloudZones()
    {
        foreach (GameObject obj in cloudList)
            obj.GetComponent<Cloud>().ResetCloudColor();

        float zoneXMin = 0;
        float zoneXMax = 0;

        if (debugRange == 1)
        {
            zoneXMin = subBassMin;
            zoneXMax = subBassMax;
        }
        else if (debugRange == 2)
        {
            zoneXMin = midBassMin;
            zoneXMax = midBassMax;
        }
        else if (debugRange == 3)
        {
            zoneXMin = lowMidMin;
            zoneXMax = lowMidMax;
        }
        else if (debugRange == 4)
        {
            zoneXMin = hiMidMin;
            zoneXMax = hiMidMax;
        }
        zoneXMin *= Equalizer.Instance.stretcher;
        zoneXMin -= Equalizer.Instance.stretcher;
        zoneXMax *= Equalizer.Instance.stretcher;
        zoneXMax -= Equalizer.Instance.stretcher;
        //    Debug.Log("xmin " + zoneXMin + " xmax  " + zoneXMax);
        foreach (GameObject obj in cloudList)
            if (obj.transform.localPosition.x > zoneXMin && obj.transform.localPosition.x < zoneXMax)
                obj.GetComponent<Cloud>().ActivateSecondary(0);

    }

    void ResetCloudList()
    {
        cloudList = new List<GameObject>();
        foreach (Transform child in cloudParent.transform)
            if (child.gameObject.name != "CloudSpawner")
                cloudList.Add(child.gameObject);
    }
}
