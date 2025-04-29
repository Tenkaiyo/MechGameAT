using UnityEngine;

public class SupplyScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Supplied Ammo and Fuel");
            other.SendMessage("SupplyFuel");
            other.SendMessage("SupplyAmmo");
            other.SendMessage("SupplyHealth");
        }
    }
}
