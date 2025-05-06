using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{

    public GunAttributes EquipedGun;
    public bool IsPlayer = false;

    private float nextTimeToFire = 0f;
    private float currentAmmo;

    public ParticleSystem MuzzleFlash;
    public Transform BulletSpawnPoint;



    [Header("Raycast")]
    public LayerMask RayLayer;
    public Transform RayTrans;

    [Header("UI")]
    public Text AmmoText;
    public RectTransform Crosshair;

    //LaserStuff
    private GameObject ImpactParticle;
    private GameObject BloodImpactParticle;
    private LineRenderer LaserLineRender;
    private Vector3 CurrTargetRot, OldTargetRot;
    private GameObject TargetObj;
    private HealthScript TargetHealth;


    void Start()
    {
        if(IsPlayer)
        {
            SupplyAmmo();
            Crosshair.sizeDelta = new Vector2(EquipedGun.FireSpread*20,EquipedGun.FireSpread*20);
        }
        OldTargetRot = RayTrans.forward;
    }

    public void Shoot()
    {
        if(EquipedGun.GunType != GunAttributes.GunTypes.Laser)
        {
            if (Time.time < nextTimeToFire || currentAmmo <= 0 && !EquipedGun.InfiniteAmmo)
            {
                return;
            }
            currentAmmo -= 1;
            UpdateAmmoUI();
            nextTimeToFire = Time.time + EquipedGun.fireRate;
            MuzzleFlash.Play();
        }

        if(EquipedGun.GunType == GunAttributes.GunTypes.Laser)
        {
            if(currentAmmo <= 0 && !EquipedGun.InfiniteAmmo)
            {
                return;
            }
            currentAmmo -= Time.deltaTime * EquipedGun.fireRate;
            UpdateAmmoUI();
        }

        Vector3 shootDirection = RayTrans.transform.forward;
        RaycastCalc(shootDirection);
    }

    public void StopShooting()
    {
        if(ImpactParticle != null)
        {
            ((ParticleSystem)ImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
            ImpactParticle = null;
        }
            
        if(BloodImpactParticle != null)
        {
            ((ParticleSystem)BloodImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
            BloodImpactParticle = null;
        }

        if(LaserLineRender != null)
        {
            Destroy(LaserLineRender.gameObject);
        }
    }

    public void ShootEnemy(Vector3 Target)
    {
        if(EquipedGun.GunType != GunAttributes.GunTypes.Laser)
        {
            if (Time.time < nextTimeToFire)
            {
                return;
            }
            nextTimeToFire = Time.time + EquipedGun.fireRate;
            StartCoroutine(ShootEnemyDelay(Target));
        }

        if(EquipedGun.GunType == GunAttributes.GunTypes.Laser)
        {  
            if (Time.time < nextTimeToFire)
            {
                //CurrTargetPos = Vector3.MoveTowards(CurrTargetPos,Target,Time.deltaTime * EquipedGun.BulletSpeed);
                CurrTargetRot = (Target+ new Vector3(0,1f,0)) - RayTrans.position;
                return;
            }
            nextTimeToFire = Time.time + EquipedGun.fireRate;
            StartCoroutine(LaserEnemyDelay());

        }
    }
    public IEnumerator ShootEnemyDelay(Vector3 Target)
    {
        yield return new WaitForSeconds(.2f);

        Vector3 shootDirection = (Target + new Vector3(0,1f,0)) - RayTrans.position;

        RaycastCalc(shootDirection);

    }

    public IEnumerator LaserEnemyDelay()
    {

        Debug.Log("LaserStart");
        Debug.Log(OldTargetRot);
        while(Time.time < nextTimeToFire){
            OldTargetRot = Vector3.MoveTowards(OldTargetRot.normalized, CurrTargetRot.normalized, Time.deltaTime * EquipedGun.BulletSpeed);
            RaycastCalc(OldTargetRot);
            yield return null;
        }
        StopShooting();
    }

    void RaycastCalc(Vector3 ShootDirection)
    {
        for (int i = 0; i < EquipedGun.BulletsPerFire; i++)
        {

            RaycastHit hit;
            Vector3 shootDirection = ShootDirection + RayTrans.TransformDirection(Random.insideUnitCircle * (EquipedGun.FireSpread / 100f));

            if (Physics.Raycast(RayTrans.transform.position, shootDirection, out hit, EquipedGun.range, RayLayer))
            {
                if(EquipedGun.GunType == GunAttributes.GunTypes.HitScan)
                {
                    Debug.Log(hit.transform.name);
                    LineRenderer Line = Instantiate(EquipedGun.BulletLine, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnLine(Line,RayTrans.transform.position, hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject, EquipedGun.ReflectionCount)); 
                }

                if(EquipedGun.GunType == GunAttributes.GunTypes.HitscanDelay)
                {
                    Debug.Log(hit.transform.name);
                    TrailRenderer trail = Instantiate(EquipedGun.BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail,RayTrans.transform.position, hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject, EquipedGun.ReflectionCount)); 
                }

                if(EquipedGun.GunType == GunAttributes.GunTypes.Laser)
                {
                    Laser(hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject);
                }
            }
            else
            {
                if(EquipedGun.GunType == GunAttributes.GunTypes.HitScan)
                {
                    Debug.Log("Hit nothing... cringe");
                    LineRenderer Line = Instantiate(EquipedGun.BulletLine, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnLine(Line,RayTrans.transform.position,RayTrans.transform.position + shootDirection * EquipedGun.range, RayTrans.transform.eulerAngles, EquipedGun.range, null, null, 0)); 
                }

                if(EquipedGun.GunType == GunAttributes.GunTypes.HitscanDelay)
                {
                    Debug.Log("Hit nothing... cringe");
                    TrailRenderer trail = Instantiate(EquipedGun.BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail,RayTrans.transform.position,RayTrans.transform.position + shootDirection * EquipedGun.range, RayTrans.transform.eulerAngles, EquipedGun.range, null, null, 0)); 
                }

                if(EquipedGun.GunType == GunAttributes.GunTypes.Laser)
                {
                    Laser(RayTrans.transform.position + shootDirection * EquipedGun.range, RayTrans.transform.eulerAngles, EquipedGun.range, null, null);
                }
            }
        }
    }



    //Hitscan Delay
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


        if(Hitbox != null && Hitbox.tag == "Hitbox")
        {
            //Hitobj.SendMessage("Damage", BulletDamage);
            float damage = CalcDamage(distance);

            ((HealthScript)Entity.GetComponent(typeof(HealthScript))).Damage(damage);
            Debug.Log("damage: " + damage);
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


    //Laser
    private void Laser(Vector3 Pos, Vector3 Rot, float distance, GameObject Hitbox, GameObject Entity)
    {
        if(LaserLineRender == null)
        {
            LaserLineRender = Instantiate(EquipedGun.BulletLine);
        }
        LaserLineRender.SetPosition(0, BulletSpawnPoint.position);
        LaserLineRender.SetPosition(1, Pos);


        if(Hitbox != null && Hitbox.tag == "Hitbox")
        {   
            if(ImpactParticle != null)
            {
                ((ParticleSystem)ImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
                ImpactParticle = null;
            }
            if(BloodImpactParticle == null)
            {
                BloodImpactParticle = Instantiate(EquipedGun.BloodParticle);
            }
            BloodImpactParticle.transform.position = Pos;
            BloodImpactParticle.transform.rotation = Quaternion.LookRotation(Rot);

            float damage = CalcDamage(distance);
            damage = damage * Time.deltaTime;

            if(TargetObj != Entity)
            {
                TargetHealth = (HealthScript)Entity.GetComponent(typeof(HealthScript));
            }
            TargetHealth.Damage(damage);
            //Instantiate(EquipedGun.BloodParticle, Pos, Quaternion.LookRotation(Rot));
        }
        else if(Hitbox != null)
        {
            if(BloodImpactParticle != null)
            {
                ((ParticleSystem)BloodImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
                BloodImpactParticle = null;
            }
            if(ImpactParticle == null)
            {
                ImpactParticle = Instantiate(EquipedGun.ImpactParticle);
            }
            ImpactParticle.transform.position = Pos;
            ImpactParticle.transform.rotation = Quaternion.LookRotation(Rot);
        }
        else
        {
            if(ImpactParticle != null)
            {
                ((ParticleSystem)ImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
                ImpactParticle = null;
            }
            
            if(BloodImpactParticle != null)
            {
                ((ParticleSystem)BloodImpactParticle.GetComponent(typeof(ParticleSystem))).Stop();
                BloodImpactParticle = null;
            }
        }
    }


    //Hitscan
    private  IEnumerator SpawnLine(LineRenderer Line, Vector3 CamPos, Vector3 Pos, Vector3 Rot, float distance,GameObject Hitbox, GameObject Entity, int Reflects)
    {
        float time = 1;
        float lineWidth = Line.startWidth;

        Line.SetPosition(0, Line.transform.position);
        Line.SetPosition(1, Pos);

        if(Hitbox != null && Hitbox.tag == "Hitbox")
        {
            float damage = CalcDamage(distance);

            ((HealthScript)Entity.GetComponent(typeof(HealthScript))).Damage(damage);
            Debug.Log("damage: " + damage);
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
                LineRenderer NewLine = Instantiate(EquipedGun.BulletLine, Pos, Quaternion.identity);

                if(Physics.Raycast(Pos, bounceDirection, out RaycastHit hit, EquipedGun.ReflectRange, RayLayer))
                {
                    StartCoroutine(SpawnLine(NewLine,Pos, hit.point, hit.normal, hit.distance, hit.collider.transform.gameObject, hit.transform.gameObject, Reflects)); 
                }
                else
                {
                    StartCoroutine(SpawnLine(NewLine,Pos, Pos + bounceDirection * EquipedGun.ReflectRange, bounceDirection, EquipedGun.ReflectRange, null, null, Reflects)); 
                }
            }
        }

        while(time > 0)
        {
            Line.startWidth = time * lineWidth;
            Line.endWidth = time * lineWidth;
            time -= Time.deltaTime * EquipedGun.BulletSpeed;
            yield return null;
        }

        Destroy(Line.gameObject);
    }


    float CalcDamage(float distance)
    {
        float Dist = 0;
        if(distance > EquipedGun.nearDistance && distance < EquipedGun.farDistance){
            Dist = (distance - EquipedGun.nearDistance) / (EquipedGun.farDistance - EquipedGun.nearDistance);
        }
        if(distance > EquipedGun.farDistance)
        {
            Dist = 1f;
        }
        float BulletDamage = Dist * EquipedGun.MinDamage + (1f - Dist) * EquipedGun.MaxDamage;
        return BulletDamage;
    }

    public void SupplyAmmo()
    {
        currentAmmo = EquipedGun.MaxAmmo;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        AmmoText.text = currentAmmo.ToString("f0");
    }
}
