using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    private List<PlayerController> jugadores = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Debug.Log("[GameManager] Server activo. Esperando jugadores...");
        PlayerController.OnPlayerSpawned += RegistrarJugador;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            PlayerController.OnPlayerSpawned -= RegistrarJugador;
    }

    private void RegistrarJugador(PlayerController player)
    {
        if (!IsServer) return;

        WeaponType arma;
        if (player.OwnerClientId == 0)
        {
            arma = WeaponType.Laser;
        }
        else
        {
            arma = WeaponType.Shotgun;
        }
        
        player.weaponType = arma;

        jugadores.Add(player);
        Debug.Log($"[GameManager] Jugador {player.OwnerClientId} asignado con arma {arma}");
    }
}
