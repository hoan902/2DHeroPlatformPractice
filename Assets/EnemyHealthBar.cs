using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fillColor;
    public TextMeshProUGUI healthCount;
    string healthInfo;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fillColor.color = gradient.Evaluate(1f);
    }
    public void SetCurrentHealth(int health)
    {
        fillColor.color = gradient.Evaluate(slider.normalizedValue);
        slider.value = health;
    }
    void Update()
    {
        healthInfo = slider.value + "/" + slider.maxValue;
        healthCount.text = healthInfo;
    }
}
