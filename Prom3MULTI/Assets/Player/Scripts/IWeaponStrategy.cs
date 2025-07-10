using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IWeaponStrategy
{
    void Shoot(Transform shootPoint, ulong ownerId, GameObject bulletPrefab);
}
