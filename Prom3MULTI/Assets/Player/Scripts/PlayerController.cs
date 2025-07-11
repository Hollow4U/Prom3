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

    private Vector3 lastDirection = Vector3.forward;

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
            FireServerRpc(lastDirection);
            fireTimer = fireCooldown;
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir.sqrMagnitude > 0.01f)
        {
            lastDirection = dir;
            shootPoint.rotation = Quaternion.LookRotation(dir);
        }

        rb.linearVelocity = dir * speed + Vector3.up * rb.linearVelocity.y;
    }

    [Rpc(SendTo.Server)]
    private void FireServerRpc(Vector3 direction)
    {
        GameObject prefab = weaponType == WeaponType.Laser ? laserBulletPrefab : shotgunBulletPrefab;

        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject bullet = Instantiate(prefab, shootPoint.position, rotation);
        bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}
