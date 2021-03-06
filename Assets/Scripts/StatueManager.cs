﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueManager : MonoBehaviour
{
    public float health;
    private float maxHealth = 100f;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        if (_damage > 0f)
            health -= _damage;

        if (health <= 0f)
        {
            // game over??
            Debug.Log("Game Over!!");
        }

        ServerSend.StatueHealth(this);
    }

    public void Heal(float _amount)
    {
        health += _amount;

        if (health > maxHealth)
            health = maxHealth;

        ServerSend.StatueHealth(this);
    }
}
