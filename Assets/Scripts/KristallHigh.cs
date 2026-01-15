using UnityEngine;

public class KristallHigh: MonoBehaviour
{
    public KristallBar bar; //Referens till ui mätaren 
    bool alreadyUsed = false; //Ser till att denna kristall bara kan aktiveras en gång

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spelare") && !alreadyUsed) //Kollar om det är spelaren som går in i kristallen
        {
            bar.CollectCrystal(); //Säger till ui att en kristall har samlats
            alreadyUsed = true; //Markerar kristallen och gör att den inte kan triggas igen
        }
    }
}
