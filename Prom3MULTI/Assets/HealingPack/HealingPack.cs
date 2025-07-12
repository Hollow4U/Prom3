using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingPack : NetworkBehaviour
{
    [SerializeField] private int healAmount = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerLife life = other.GetComponent<PlayerLife>();
            if (life != null)
            {
                life.Heal(healAmount);
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
