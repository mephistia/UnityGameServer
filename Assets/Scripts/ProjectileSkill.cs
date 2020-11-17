using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkill : Projectile
{
    protected override void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        rb2d.AddForce(direction.normalized * speed);
        ServerSend.SpawnProjectileSkill(this, byPlayer);
    }

    protected override void DoDamage(Collider2D _collider) // dar override no script de projétil do inimigo
    {
        if (_collider.gameObject.CompareTag("Enemy"))
        {
            Enemy _enemy = _collider.gameObject.GetComponent<Enemy>();
            _enemy.TakeDamage(damage);
            // corrigir?????
            _enemy.rb2d.isKinematic = false;
            _enemy.rb2d.AddForce((direction.normalized * speed) * 4f);
            _enemy.agent.isStopped = true;
            collisionTag = "Enemy";
        }
        else
            collisionTag = "Object";

        ServerSend.ProjectileDamaged(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // send projectileskillposition?
    }

}
