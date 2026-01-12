using UnityEngine;

public class Kristall : MonoBehaviour
{
    //Referens för spelaren
    public KristallBar kristallBar;


    //När spelaren går in i kristallens fysik (trigger)
    void  OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("?????"))
        {
            kristallBar.SamlaKristall();  //Lägga till kristall till baren/mätaren


            Destroy(gameObject); // Tar bort kristallen från scenen
        }
    }   
}
