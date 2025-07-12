using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int maxHealth;

    [SerializeField]private int currentHealth;
    private Transform target;

    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private float dropChance = 0.3f; 

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!IsServer) return;

        FindClosestPlayer();

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    private void FindClosestPlayer()
    {
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = player.transform;
            }
        }

        target = closest;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TryDropHealth();
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Bullet"))
        {
            TakeDamage(5);
        }
        else if (other.CompareTag("SBullet"))
        {
            TakeDamage(10);
        }
        else if (other.CompareTag("Player"))
        {
            PlayerLife life = other.GetComponent<PlayerLife>();
            if (life != null)
            {
                life.TakeDamageServerRpc(1);
            }
        }
    }

    private void TryDropHealth()
    {
        if (healthPickupPrefab == null) return;

        if (UnityEngine.Random.value < dropChance)
        {
            GameObject pickup = Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
            pickup.GetComponent<NetworkObject>().Spawn();
        }
    }
}
