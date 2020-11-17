using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public Vector2 velocity;
    public Rigidbody2D rb2d;
    public Transform shootOrigin;
    public float health;
    public float maxHealth = 100f;
    [SerializeField]
    private float moveSpeed = 300f; // deixar igual no client para predict!

    public int energyCount = 0;

    protected float GetMoveSpeed()
    {
        return moveSpeed;
    }

    protected void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    public virtual void TakeDamage(float _damage)
    {
        // não recebe dano depois de morto
        if (health <= 0f)
            return;

        health -= _damage;

        // se nessa execução morreu:
        if (health <= 0f)
        {
            // matar o personagem
            //... animação?
            Destroy(gameObject);
        }
    }
}
