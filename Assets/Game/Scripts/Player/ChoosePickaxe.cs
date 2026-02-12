using UnityEngine;

public class ChoosePickaxe : MonoBehaviour
{
    [HideInInspector]public Tool chosenPickaxe;
    [SerializeField] private Tool pickAxe;
    [SerializeField] private Tool pickPipe;
    void Awake()
    {
        if(Random.Range(0,100f) <= 5)
        {
            chosenPickaxe = pickPipe;
            pickAxe.gameObject.SetActive(false);
        }
        else
        {
            chosenPickaxe = pickAxe;
            pickPipe.gameObject.SetActive(false);
        }
        print(chosenPickaxe);
    }

}
