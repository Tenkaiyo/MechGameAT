using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{

    public GunAttributes EquipedGun;
    public bool IsPlayer = false;

    private float nextTimeToFire = 0f;
    private int currentAmmo;

    public ParticleSystem MuzzleFlash;
    public Transform BulletSpawnPoint;



    [Header("Raycast")]
    public LayerMask RayLayer;
    public Transform RayTrans;

    [Header("UI")]
    public Text AmmoText;
    public RectTransform Crosshair;

    void Start()
    {
        if(IsPlayer)
        {
            SupplyAmmo();
            Crosshair.sizeDelta = new Vector2(EquipedGun.FireSpread*20,EquipedGun.FireSpread*20);
        }
    }

    public void Shoot()
    {
        if (Time.time < nextTimeToFire || currentAmmo <= 0 && !EquipedGun.InfiniteAmmo)
        {
            return;
        }
        currentAmmo -= 1;
        UpdateAmmoUI();
        nextTimeToFire = Time.time + EquipedGun.fireRate;
        MuzzleFlash.Play();
        Vector3 shootDirection = RayTrans.transform.forward;

        RaycastCalc(shootDirection);
    }


    public void ShootEnemy(Vector3 Target)
    {
        if (Time.time < nextTimeToFire)
        {
            return;
        }
        nextTimeToFire = Time.time + EquipedGun.fireRate;

        StartCoroutine(ShootEnemyDelay(Target));
    }

    public IEnumerator ShootEnemyDelay(Vector3 Target)
    {
        yield return new WaitForSeconds(.2f);

        Vector3 shootDirection = (Target + new Vector3(0,1f,0)) - RayTrans.position;

        RaycastCalc(shootDirection);

    }


    void RaycastCalc(Vector3 shootDirection)
    {
        for (int i = 0; i < EquipedGun.BulletsPerFire; i++)
        {

            RaycastHit hit;
            shootDirection = shootDirection + RayTrans.TransformDirection(Random.insideUnitCircle * (EquipedGun.FireSpread / 100f));

            if (Physics.Raycast(RayTrans.transform.position, shootDirection, out hit, EquipedGun.range, RayLayer))
            {
                Debug.Log(hit.transform.name);

                if(EquipedGun.GunType == GunAttributes.GunTypes.HitscanDelay)
                {
                    TrailRenderer trail = Instantiate(EquipedGun.BulletTrail,BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail,RayTrans.transform.position, hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject, EquipedGun.ReflectionCount)); 
                }
            }
            else
            {
                Debug.Log("Hit nothing... cringe");

                if(EquipedGun.GunType == GunAttributes.GunTypes.HitscanDelay)
                {
                    TrailRenderer trail = Instantiate(EquipedGun.BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail,RayTrans.transform.position,RayTrans.transform.position + shootDirection * EquipedGun.range, RayTrans.transform.eulerAngles, EquipedGun.range, null, null, EquipedGun.ReflectionCount)); 
                }
            }
        }
    }


    private IEnumerator SpawnTrail(TrailRenderer Trail,Vector3 CamPos, Vector3 Pos, Vector3 Rot, float distance, GameObject Hitbox, GameObject Entity, int Reflects)
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
        #endregion


        if(Hitbox != null && Hitbox.tag == "Hitbox")
        {
            //Hitobj.SendMessage("Damage", BulletDamage);
            ((HealthScript)Entity.GetComponent(typeof(HealthScript))).Damage(BulletDamage);
            Debug.Log(Dist + "danage" + BulletDamage);
            Instantiate(EquipedGun.BloodParticle, Pos, Quaternion.LookRotation(Rot));
        }
        else if(Hitbox != null)
        {
            Instantiate(EquipedGun.ImpactParticle, Pos, Quaternion.LookRotation(Rot));
            if(Reflects > 0)
            {
                Vector3 direction = (Pos - CamPos).normalized;
                Vector3 bounceDirection = Vector3.Reflect(direction,Rot);
                Reflects -= 1;

                if(Physics.Raycast(Pos, bounceDirection, out RaycastHit hit, EquipedGun.ReflectRange, RayLayer))
                {
                    StartCoroutine(SpawnTrail(Trail,Trail.transform.position, hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject, Reflects)); 
                }
                else
                {
                    StartCoroutine(SpawnTrail(Trail,Trail.transform.position, Pos + bounceDirection * EquipedGun.ReflectRange, bounceDirection, EquipedGun.ReflectRange, null, null, Reflects)); 
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
