using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class KristallBar
{
    public Slider slider;
    public float fillSpeed = 1.5f;

    float currentValue = 0f;
    float targetValue = 0f;

    int crystalsCollected = 0;

    void Start()
    {
        slider.value = 0f;
    }
    void Update()
    {
        if (currentValue < targetValue)
        {
            currentValue += fillSpeed * Time.deltaTime;
            currentValue = Mathf.Min(currentValue, targetValue);
            slider.value = currentValue;
        }
    }

    public void CollectCrystal()
    {
        crystalsCollected++;

        if (crystalsCollected == 1)
            targetValue = 0.5f;

        else if (crystalsCollected == 2)
            targetValue = 1f;
    }

}




   







