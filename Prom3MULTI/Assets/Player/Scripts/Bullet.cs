using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    private float timer;

    private void Start()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        if (!IsServer) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            NetworkObject.Despawn();
        }
    }
}
