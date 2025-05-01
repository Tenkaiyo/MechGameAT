using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{

    public float MaxHealth, CurrHealth;
    public Slider HealthSlider;

    void Start()
    {
        CurrHealth = MaxHealth;

        if(HealthSlider != null)
        {
            HealthSlider.maxValue = MaxHealth;
            UpdateHealthUI();
        }
    }

    public void Damage(float damage)
    {
        CurrHealth -= damage;
        UpdateHealthUI();

        if(CurrHealth <= 0){
            CurrHealth = 0;
            if(gameObject.tag != "Player")
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
                return;
            }

            Debug.Log("You died");
        }
    }

    public void SupplyHealth()
    {
        CurrHealth = MaxHealth;
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if(HealthSlider != null)
        {
            HealthSlider.value = CurrHealth;
        }
    }
}
