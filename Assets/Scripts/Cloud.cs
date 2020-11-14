using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{

    float colorTint;
    public bool timedCloud;
    // Start is called before the first frame update
    void Start()
    {
        if (timedCloud == true)
             Destroy(gameObject, WeatherController.Instance.cloudRestoreRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateCloud(int indexInList )
    {
        // check the total size of list

        // gradient across that size

        // 4 extra clouds
        // 
        float incrementPerIndex = 1f / (WeatherController.Instance.cloudNeighborAmount - 1);

        colorTint = incrementPerIndex * indexInList; 

        if (indexInList == 0)
        {
           GetComponent<SpriteRenderer>().color = new Color (0,0,0,255); 
        }
        else
        { 
            GetComponent<SpriteRenderer>().color = new Color(colorTint, colorTint, colorTint, 255);
            //   GetComponent<SpriteRenderer>().color = WeatherController.Instance.cloudDark;
            //   GetComponent<SpriteRenderer>().color = new Color(111f, 111f, 111f, 255);
        }
      //  StartCoroutine(ColorCountdown());
    }

    public void ActivateSecondary(int indexInList)
    {
        float incrementPerIndex = 1f / (WeatherController.Instance.cloudZoneNeighborAmount - 1);

        colorTint = incrementPerIndex * indexInList;
      

        if (colorTint < 0.3f)
            colorTint = 0.3f;
    //    GetComponent<SpriteRenderer>().color = new Color(colorTint, colorTint, colorTint, 255);
        GetComponent<SpriteRenderer>().color = new Color(0.55f, 0.55f, 0.55f, 255);

    }

    public IEnumerator ColorCountdown()
    {
        for (float i = colorTint; i <1 ; i+= WeatherController.Instance.cloudRestoreRate)
        {
            
            GetComponent<SpriteRenderer>().color = new Color(i, i, i, 255);
            yield return null;
        }
        colorTint = 1;
       

    }
}
