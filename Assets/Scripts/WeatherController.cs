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
    public enum Threshold { Low, Mid, High }

    [Header("Ranges")]
    public float subBaseMin;
    public float subBaseMax;
    public float midBassMin;
    public float midBassMax;
    public float lowMidMin;
    public float lowMidMax;
    public float hiMidMin;
    public float hiMidMax;



    [Header("Thresholds")] // Going by multiplier 10, volume 0.5
    public float lowThreshold;
    public float midThreshold;
    public float hiThreshold;


    [Header("Colors")]
    public Color cloudDefault;
    public Color cloudDark;
    public Color cloudDarkest;

    [Header("Clouds")]
    public float cloudRestoreRate;
    public int cloudLessener;
    public int cloudColorStick;
    public int cloudNeighborAmount;
    public int cloudZoneNeighborAmount;
    public bool zoneClouds;
    public bool resetColors;
    public Threshold cloudSpawnThreshold;

    [Header("Rain")]
    public float rainThreshold;
    public float snowThreshold;

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
    private GameObject nextHighestCloud;
    private List<GameObject> foundClouds;
    private List<GameObject> foundZoneClouds;

    private Dictionary<float, float> zoneMapping;
    private void Awake()
    {
        Instance = this;
        ResetCloudList();

        highestCloud = cloudList[0];



    }
    // Update is called once per frame
    void Update()
    {

        FindHighValues();
   
        CheckForWeatherChanges();
        ResetHighValues();
    }
 
    void CheckForWeatherChanges()
    {
        ChangeCloudColors();
        if (zoneClouds == true)
            FindZoneClouds();
        RainController.Instance.SpawnRain();
   //     RainController.Instance.SpawnSnow();

                   
        //    CheckForCloudSpawn();
    }
    void FindHighValues()
    {
        zoneMapping = new Dictionary<float, float>();
        zoneMapping.Add(subBaseMin, subBaseMax);
        zoneMapping.Add(midBassMin, midBassMax);
        zoneMapping.Add(lowMidMin, lowMidMax);
        zoneMapping.Add(hiMidMin, hiMidMax);
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
            highestZonePeakX *= Equalizer.Instance.stretcher;
            highestZonePeakX -= Equalizer.Instance.stretcher;



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
            Debug.Log("  x map " + zoneMin+ "  size of list" + foundZoneClouds.Count);

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
        if (highestPeak > lowThreshold)
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
        


            {
                //  Debug.Log("MAX color changed! " + closestCloud.transform.localPosition.x);


                //int cloudIndex = cloudList.IndexOf(highestCloud);            
                //GameObject cloudLeft = cloudList[cloudIndex - 1];
                //GameObject cloudRight = cloudList[cloudIndex + 1];
                //cloudLeft.GetComponent<SpriteRenderer>().color = cloudDark;
                //cloudRight.GetComponent<SpriteRenderer>().color = cloudDark;

                if (Random.Range(0, cloudColorStick) == 0)
                    ResetCloudColors();


            }

            //else
            //    ResetCloudColors();



        }
        void ResetCloudColors()
        {
            if (resetColors == true)
                foreach (GameObject obj in cloudList)
                    obj.GetComponent<SpriteRenderer>().color = cloudDefault;
        }
        void CheckForCloudSpawn()
        {
            if (cloudSpawnThreshold == Threshold.Low)
            {
                if (CheckRange(subBaseMin, subBaseMax, lowThreshold) == true)
                    SpawnCloud();
            }
            else if (cloudSpawnThreshold == Threshold.Mid)
            {
                if (CheckRange(subBaseMin, subBaseMax, midThreshold) == true)
                    SpawnCloud();
            }
            else if (cloudSpawnThreshold == Threshold.High)
            {
                if (CheckRange(subBaseMin, subBaseMax, hiThreshold) == true)
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
                cloudObj.GetComponent<SpriteRenderer>().color = new Color(tint, tint, tint, Random.Range(0.4f, 1f));
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
    void ResetCloudList()
    {
        cloudList = new List<GameObject>();
        foreach (Transform child in cloudParent.transform)
            if (child.gameObject.name != "CloudSpawner")
                cloudList.Add(child.gameObject);
    }
}
