using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equalizer : MonoBehaviour
{

   

    // TODO 
    // make a CheckForCloud function, dark cloud on sustained bass. 
    // Create specific ranges, and specific time-thresholds. Check in above-like functions for combinations of these.
   // make a rain fucntion using particle
    public static Equalizer Instance;


    [Header("Audio")]
    public int sampleRate;  
    public float heightMultiplier;
    public float stretcher; 


    private float[] spectrum ;
    private Dictionary<float,float> frequencyMap;

    private void Awake()
    {
        Instance = this;
        frequencyMap = new Dictionary<float,float>();
        spectrum = new float[Equalizer.Instance.sampleRate];

    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        if (WeatherController.Instance.showHeightIndicator == true)
        Debug.DrawLine(new Vector3(0, WeatherController.Instance.heightIndicatorHeight*stretcher-10, 1), new Vector3(80, WeatherController.Instance.heightIndicatorHeight*stretcher-10, 1), Color.red);


        for (int i = 1; i < spectrum.Length - 1; i++)        
            spectrum[i] *= heightMultiplier;

        frequencyMap = new Dictionary<float, float>();
            for (int i = 1; i < spectrum.Length - 1; i++)
        {
            frequencyMap.Add(Mathf.Log10(i), spectrum[i]);
        //   Debug.DrawLine(new Vector3(-Mathf.Log10(i - 1), (spectrum[i - 1] - 10) * heightMultiplier, 1), new Vector3(-Mathf.Log10(i), (spectrum[i] - 10) * heightMultiplier, 1), Color.green);
                Debug.DrawLine(new Vector3(Mathf.Log10(i - 1)*stretcher - (stretcher), spectrum[i - 1] * stretcher - 10, 1), new Vector3(Mathf.Log10(i) * stretcher - (stretcher), spectrum[i] * stretcher - 10, 1), Color.green);
        }

    }


    public float[] GetSpectrum()
    { 
            return spectrum; 
    }
    public Dictionary<float, float> GetFrequencyMap()
    {
        return frequencyMap;
    }

}
