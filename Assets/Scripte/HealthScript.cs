using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{

    public float MaxHealth, CurrHealth;
    public Slider HealthSlider;
    public Image HurtUI;
    public MonoBehaviour EnemyScript;

    float HurtTimer;
    bool Dead = false;
    private int interval = 30;

    void Start()
    {
        CurrHealth = MaxHealth;

        if(HealthSlider != null)
        {
            HealthSlider.maxValue = MaxHealth;
            UpdateHealthUI();
        }
    }

    void Update()
    {
        if (Time.frameCount % interval == 0)
        { 
            if(HurtTimer > 0)
            {
                HurtTimer = Mathf.Clamp(HurtTimer, 0.0f, 1.0f);
                HurtTimer -= Time.deltaTime / 2 * interval;
                HurtUI.color = new Color(1f,1f,1f,HurtTimer);
            }
        }
    }

    public void Damage(float damage)
    {
        if(CurrHealth == MaxHealth && EnemyScript != null)
        {
            EnemyScript.SendMessage("GotHit");
        }

        CurrHealth -= damage;
        Hurt(damage);
        UpdateHealthUI();

        if(CurrHealth <= 0 && !Dead){
            Dead = true;
            CurrHealth = 0;
            if(EnemyScript != null)
            {
                EnemyScript.SendMessage("Die");
            }
            else if(gameObject.tag != "Player")
            {
                gameObject.SetActive(false);
            }
            if(gameObject.tag == "Player")
            {
                SendMessage("Die");
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

    void Hurt(float damage)
    {
        if(HurtUI != null)
        {
            HurtTimer += damage / 50f;
        }
    }

}
