using System.Collections;
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
        health -= (_damage / 10f); //teste

        if (health <= 0f)
        {
            // game over??
            Debug.Log("Game Over!!");
        }

        ServerSend.StatueHealth(this);
    }
}
