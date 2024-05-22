using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject bulletFX;
    public Transform aimTarget;
    public Camera aimTarget2;
    public Vector3 aaaaa;

    public void ShootWeapon()
    {
        GameObject bullet = Instantiate(bulletFX, aimTarget.position, aimTarget.rotation);
        Ray ray = aimTarget2.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint = ray.GetPoint(1000);
        Vector3 direction = (targetPoint - aimTarget.position).normalized;
        aaaaa = direction;
        //GameObject bullet = Instantiate(bulletFX, aimTarget.position, aimTarget.rotation);
    }
}

