using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
    Laser,
    Shotgun
}

public class PlayerController : NetworkBehaviour
{
    public static event System.Action<PlayerController> OnPlayerSpawned;

    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject laserBulletPrefab;
    [SerializeField] private GameObject shotgunBulletPrefab;

    public WeaponType weaponType;

    private IWeaponStrategy currentStrategy;
    private float fireCooldown = 0.2f;
    private float fireTimer = 0f;

    private void Awake()
    {
        currentStrategy = new ShotgunWeapon();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            AssignStrategy(weaponType);
        }

        if (IsServer)
        {
            OnPlayerSpawned?.Invoke(this);
        }
    }

    private void AssignStrategy(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Laser:
                currentStrategy = new LaserWeapon();
                fireCooldown = 0.1f;
                break;
            case WeaponType.Shotgun:
                currentStrategy = new ShotgunWeapon();
                fireCooldown = 0.8f;
                break;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        Move();

        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            FireServerRpc();
            fireTimer = fireCooldown;
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        rb.linearVelocity = dir * speed + Vector3.up * rb.linearVelocity.y;
    }

    [Rpc(SendTo.Server)]
    private void FireServerRpc()
    {
        GameObject prefab = weaponType == WeaponType.Laser ? laserBulletPrefab : shotgunBulletPrefab; 
         currentStrategy.Shoot(shootPoint, OwnerClientId, prefab);
    }
}
