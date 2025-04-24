using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float MaxHealth, CurrHealth;

    void Start()
    {
        CurrHealth = MaxHealth;
    }

    public void Damage(float damage)
    {
        CurrHealth -= damage;
        if(CurrHealth <= 0){
            Destroy(gameObject);
        }
    }
}
