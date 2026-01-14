using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class KristallBar
{
    public Slider slider;  //UI mätaren 
    public float fillSpeed = 1.5f; // Hur snabbt den fylls

    float currentValue = 0f; // Vad baren är på just nu
    float targetValue = 0f; //Hur mycket den ska fyllas upp till

    int crystalsCollected = 0; //Antal kristaller spelaren tagit 

    float drainTimer = 0f; //Hur lång tid högheten ska räcka
    float drainDuration = 0f; //30 eller 60 sek beroende på antal kristaller
    void Start()
    {
        slider.value = 0f;  //Mätaren startas tom
    }
    void Update()
    {   // Fyllning på baren blir smooth

        if (currentValue < targetValue)
        {
            currentValue += fillSpeed * Time.deltaTime;
        }
        //Timern räknas ner när baren fylls 
        if (drainTimer > 0)
        {
            drainTimer -= Time.deltaTime;
            float t = drainTimer / drainDuration;  //Hur mycket progress som ska vara kvar
            targetValue = Mathf.Clamp01(t);
        }
        else
        {  //När tiden är slut blir baren tom igen
            targetValue = 0f;
            crystalsCollected = 0;
        }

        currentValue = Mathf.MoveTowards(currentValue, targetValue, fillSpeed * Time.deltaTime);
        slider.value = currentValue;
    }

    public void CollectCrystal()
    {
        crystalsCollected++;

        if (crystalsCollected == 1)
        {
            targetValue = 0.5f;
            drainDuration = 30f; //30 sek tills baren blir tom
            drainTimer = 30f;
        }
        else if (crystalsCollected == 2)
        {
            targetValue = 1f;
            drainDuration = 60f; //60 sek tills baren blir tom
            drainTimer = 60f;
        }
        

        
            
    }

}




   







