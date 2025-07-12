using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLife : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth = maxHealth;
        }
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(int damage)
    {
        if (!IsServer) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    public void Heal(int amount)
    {
        if (!IsServer) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log($"Player {OwnerClientId} healed. Current HP: {currentHealth}");
    }
}
