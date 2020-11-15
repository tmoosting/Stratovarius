using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    public static RainController Instance;

    public ParticleSystem rainSystem;
    public ParticleSystem rainSystem2;
    public ParticleSystem snowSystem;

    float totalBassZonesActivity;
    float totalMidZonesActivity;
    bool isRaining = false;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        snowSystem.Stop();
        rainSystem.Stop();
        rainSystem2.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnSnow()
    {
        if (isRaining == false)
        {
            int locAmount = 0;
            foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
            {
                if (xValue > WeatherController.Instance.lowMidMin && xValue < WeatherController.Instance.hiMidMax)
                {
                    locAmount++;
                    totalMidZonesActivity += Equalizer.Instance.GetFrequencyMap()[xValue];
                }
            }
            float averageMidActivity = totalMidZonesActivity / locAmount;
            Debug.Log("avg mid activity" + averageMidActivity);
            if (averageMidActivity > WeatherController.Instance.snowThreshold)            
                snowSystem.Play();            
            else
                snowSystem.Stop();
            totalMidZonesActivity = 0;
        }
        else
            snowSystem.Stop();
    }
    public void SpawnRain()
    {
        int locAmount = 0;
        totalBassZonesActivity = 0;
        foreach (float xValue in Equalizer.Instance.GetFrequencyMap().Keys)
        {
            if (xValue > WeatherController.Instance.subBassMin && xValue < WeatherController.Instance.midBassMax)
            {
                locAmount++;
                totalBassZonesActivity += Equalizer.Instance.GetFrequencyMap()[xValue];
            }
        }
        float averageBassActivity = totalBassZonesActivity / locAmount;
        if (averageBassActivity > WeatherController.Instance.rainThreshold)
        {
            isRaining = true;
            rainSystem.Play();
        }
        else
        {
            isRaining = false ;
            rainSystem.Stop();
        }
        if (averageBassActivity > WeatherController.Instance.rainThreshold * 2)
        { 
            rainSystem2.Play();
        }
        else
        { 
            rainSystem2.Stop();
        }
         
    }
}