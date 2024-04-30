using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject bulletFX;
    public Transform aimTarget;

    public void ShootWeapon()
    {
        GameObject bullet = Instantiate(bulletFX, aimTarget.position, aimTarget.rotation);
        
    }
}

