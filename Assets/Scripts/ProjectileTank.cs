using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTank : Projectile
{
    public float strength = 0f;

    protected override void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        float actualSpeed = speed * Mathf.Clamp(strength, 0.3f, 1); // velocidade mínima não pode ser muito baixa

        rb2d.AddForce(direction.normalized * actualSpeed);
        ServerSend.SpawnProjectileTank(this, byPlayer);
    }
    public void Initialize(Vector3 _direction, int _byPlayer, float _strength)
    {
        direction = _direction;
        byPlayer = _byPlayer;
        strength = _strength;
    }

    protected override void DoDamage(Collider2D _collider)
    {
        base.DoDamage(_collider);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
