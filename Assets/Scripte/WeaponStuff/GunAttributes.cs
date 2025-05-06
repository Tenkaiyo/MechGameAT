using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Ranged Weapon")]
public class GunAttributes : ScriptableObject
{

    public GunTypes GunType;
    public enum GunTypes{
        HitScan,
        HitscanDelay,
        Projectile,
        Laser,
    }

    [Header("Gun Stats")]
    public float MinDamage = 2f;
    public float MaxDamage = 10f;
    public float nearDistance, farDistance;
    [Space]
    public float range = 100f;
    public int ReflectionCount = 0;
    public float ReflectRange = 10f;
    [Space]
    public float fireRate = .1f;
    public float BulletSpeed = 150f;
    public float FireSpread = 1f;
    [Space]
    public int BulletsPerFire = 1;
    public int MaxAmmo = 30;
    public int ClipAmmo = 15;
    public bool InfiniteAmmo = false;
    

    [Header("Gun Prefabs")]
    public GameObject ImpactParticle;
    public GameObject BloodParticle;
    public TrailRenderer BulletTrail;
    public LineRenderer BulletLine;
    //public GameObject MuzzleFlash;

}
