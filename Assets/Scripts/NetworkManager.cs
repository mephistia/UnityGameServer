﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    public GameObject projectileSkillPrefab;
    public GameObject projectileTankPrefab;
    public GameObject energyPrefab;

    private void Awake()
    {
        // script único
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0; // otmimizar processo do server
        Application.targetFrameRate = 30;
       
        // max players, port
        Server.Start(2, 5020);
        
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer(int _id)
    {
        Vector3 _position = (_id == 1) ? new Vector3(-6.5f, 0, 0) : new Vector3(6.5f, 0, 0);
        Player _player = Instantiate(playerPrefab, _position, Quaternion.identity).GetComponent<Player>();
        _player.maxHealth = (_id == 1) ? 85f : 130f;
        return _player;
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.up * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }

    public ProjectileSkill InstantiateProjectileSkill(Transform _shootOrigin)
    {
        return Instantiate(projectileSkillPrefab, _shootOrigin.position + _shootOrigin.up * 0.7f, Quaternion.identity).GetComponent<ProjectileSkill>();
    }

    public ProjectileTank InstantiateProjectileTank(Transform _shootOrigin)
    {
        return Instantiate(projectileTankPrefab, _shootOrigin.position + _shootOrigin.up * 0.7f, Quaternion.identity).GetComponent<ProjectileTank>();
    }


    public Enemy InstantiateEnemy(Vector3 _position)
    {
        return Instantiate(enemyPrefab, _position, Quaternion.identity).GetComponent<Enemy>();
    }

    public Energy InstantiateEnergy(Vector3 _position)
    {
        return Instantiate(energyPrefab, _position, Quaternion.identity).GetComponent<Energy>();
    }
}

public static class Extensions
{
    public static bool TryGetComponent<T>(this GameObject obj, T result) where T : Component
    {
        return (result = obj.GetComponent<T>()) != null;
    }
}