using Unity.VisualScripting;
using UnityEngine;

public class SupplyScript : MonoBehaviour
{
    public float CoolDownTime = 20f;
    private float Timer;
    private int interval = 15;
    private MeshRenderer Meshrend;


    void Awake()
    {
        Meshrend = ((MeshRenderer)this.GetComponent(typeof(MeshRenderer)));
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && Timer <= 0)
        {
            Debug.Log("Supplied Ammo and Fuel");
            other.SendMessage("SupplyFuel");
            other.SendMessage("SupplyAmmo");
            other.SendMessage("SupplyHealth");
            Timer = CoolDownTime;
            Meshrend.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && Timer <= 0)
        {
            Debug.Log("Supplied Ammo and Fuel");
            other.SendMessage("SupplyFuel");
            other.SendMessage("SupplyAmmo");
            other.SendMessage("SupplyHealth");
            Timer = CoolDownTime;
            Meshrend.enabled = false;
        }
    }

    void Update()
    {
        if (Time.frameCount % interval == 0)
        {
            if(Timer > 0)
            {
                Timer -= Time.deltaTime * interval;
                if(Timer <= 0)
                {
                    Meshrend.enabled = true;
                }
            }
        }
    }
}
