using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    public float projectileSpeed = 5f;
    private float maxDistance = 100f;
    private float CurrDistance;
    private GameObject OriginalShooter;


    // Update is called once per frame
    void Update()
    {
        transform.Translate (Vector3.forward * projectileSpeed * Time.deltaTime);
        CurrDistance += Time.deltaTime * projectileSpeed;

        if(CurrDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject == OriginalShooter)
        {
            return;
        }

        if(other.tag == "Hitbox")
        {
            Debug.Log("Dayum");
        }

        Destroy(gameObject);
    }
}
