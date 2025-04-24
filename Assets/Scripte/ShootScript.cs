using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{

    public GunAttributes EquipedGun;

    private float nextTimeToFire = 0f;
    private int currentAmmo;

    public ParticleSystem MuzzleFlash;
    public Transform BulletSpawnPoint;



    [Header("Raycast")]
    public LayerMask RayLayer;
    public Transform CameraTrans;

    [Header("UI")]
    public Text AmmoText;

    void Start()
    {
        SupplyAmmo();
    }

    public void Shoot()
    {
        if (Time.time < nextTimeToFire || currentAmmo <= 0)
        {
            return;
        }
        currentAmmo -= 1;
        UpdateAmmoUI();
        nextTimeToFire = Time.time + EquipedGun.fireRate;
        MuzzleFlash.Play();

        for (int i = 0; i < EquipedGun.BulletsPerFire; i++)
        {
            RaycastHit hit;

            Vector3 shootDirection = CameraTrans.transform.forward;
            shootDirection = shootDirection + CameraTrans.TransformDirection(Random.insideUnitCircle * (EquipedGun.FireSpread / 100f));

            if (Physics.Raycast(CameraTrans.transform.position, shootDirection, out hit, EquipedGun.range, RayLayer))
            {
                Debug.Log(hit.transform.name);

                TrailRenderer trail = Instantiate(EquipedGun.BulletTrail,BulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail,CameraTrans.transform.position, hit.point, hit.normal, hit.distance, hit.transform.gameObject)); 

            }
            else
            {
                /*GameObject SmokeLine = Instantiate(Shotline);
                SmokeLine.GetComponent<LineRenderer>().SetPosition(0, GunFrontTrans.position);
                SmokeLine.GetComponent<LineRenderer>().SetPosition(1, RayTrans.transform.position + shootDirection * 100);*/
                Debug.Log("Hit nothing... cringe");

                TrailRenderer trail = Instantiate(EquipedGun.BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail,CameraTrans.transform.position,CameraTrans.transform.position + shootDirection * EquipedGun.range, CameraTrans.transform.eulerAngles, EquipedGun.range, null)); 
            }
        }
    }


    private IEnumerator SpawnTrail(TrailRenderer Trail,Vector3 CamPos, Vector3 Pos, Vector3 Rot, float distance, GameObject Hitobj)
    {
        #region MoveBullet
        float time = 0;
        Vector3 StartPosition = Trail.transform.position;
        while(time < 1)
        {
            Trail.transform.position = Vector3.Lerp(StartPosition, Pos, time);
            time += Time.deltaTime * EquipedGun.BulletSpeed / distance;
            yield return null;
        }
        Trail.transform.position = Pos;
        #endregion


        #region Calculate Damage
        float Dist = 0;
        if(distance > EquipedGun.nearDistance && distance < EquipedGun.farDistance){
            Dist = (distance - EquipedGun.nearDistance) / (EquipedGun.farDistance - EquipedGun.nearDistance);
        }
        if(distance > EquipedGun.farDistance)
        {
            Dist = 1f;
        }
        float BulletDamage = Dist * EquipedGun.MinDamage + (1f - Dist) * EquipedGun.MaxDamage;
        Debug.Log(Dist + "" + BulletDamage);
        #endregion


        if(Hitobj != null && Hitobj.tag == "Hitbox")
        {
            Hitobj.SendMessage("Damage", BulletDamage);
            Instantiate(EquipedGun.BloodParticle, Pos, Quaternion.LookRotation(Rot));
        }
        else if(Hitobj != null)
        {
            Instantiate(EquipedGun.ImpactParticle, Pos, Quaternion.LookRotation(Rot));
            if(EquipedGun.ReflectionCount > 0)
            {
                Vector3 direction = (Pos - CamPos).normalized;
                Vector3 bounceDirection = Vector3.Reflect(direction,Rot);

                if(Physics.Raycast(Pos, bounceDirection, out RaycastHit hit, EquipedGun.ReflectRange, RayLayer))
                {
                    StartCoroutine(SpawnTrail(Trail,Trail.transform.position, hit.point, hit.normal, hit.distance, hit.transform.gameObject)); 
                }
                else
                {
                    StartCoroutine(SpawnTrail(Trail,Trail.transform.position, Pos + bounceDirection * EquipedGun.ReflectRange, bounceDirection, EquipedGun.ReflectRange, null)); 
                }
                yield break;
            }
        }
        
        Destroy(Trail.gameObject, Trail.time);
        

    }

    public void SupplyAmmo()
    {
        currentAmmo = EquipedGun.MaxAmmo;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        AmmoText.text = currentAmmo + "";
    }
}
