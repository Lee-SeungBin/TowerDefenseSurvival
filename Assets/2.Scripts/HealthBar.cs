using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void SetHealth(float health)
    {
        healthSlider.value = health;
        healthSlider.maxValue = health;
    }
    
    public void UpdateHealth(float health)
    {
        gameObject.SetActive(true);
        healthSlider.value = health;
    }
}
