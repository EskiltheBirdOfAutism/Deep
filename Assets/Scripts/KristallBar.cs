using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class KristallBar
{

    public Image bar;  // ui baren 
    public float barFyllnad = 0f; //När kristallen blir fylld
    float tid = 0f; // Tiden sen dne senaste kristall
    bool aktiv = false;  //Ser till att baren är igång


    void Update()
    {
        if (aktiv)
        {
            tid += Time.deltaTime;

            if (tid > 30f)  // 30 sek sen går det bort
            {
                barFyllnad -= Time.deltaTime * 0.5f;

                if (barFyllnad <= 0)
                {
                    barFyllnad = 0;
                    aktiv = false;
                }
            }
        }
        bar.fillAmount = barFyllnad;  //Uppdatera baren
    }

    public void SamlaKristall() 
    {
        barFyllnad += 0.5f;   //Fyller halva baren 
        if (barFyllnad > 1f) barFyllnad = 1f;

        tid = 0f;     //startar om efter 30 sek
        aktiv = true;

    }
}


   







