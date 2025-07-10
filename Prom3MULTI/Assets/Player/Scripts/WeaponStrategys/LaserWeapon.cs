using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LaserWeapon : IWeaponStrategy
{
    public void Shoot(Transform shootPoint, ulong ownerId, GameObject bulletPrefab)
    {
        Debug.Log($"test1");
        var bullet = GameObject.Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}
