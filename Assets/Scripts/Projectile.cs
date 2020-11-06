using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public int byPlayer;
    public Rigidbody2D rb2d;
    public Vector3 direction;
    public float speed = 300f;
    public float damage = 25f;
    public float bulletRadius = 0.09f;
    private float timeToDestroy = 10f;

    protected virtual void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        rb2d.AddForce(direction.normalized * speed);
        ServerSend.SpawnProjectile(this, byPlayer);
    }

    void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);

        // destruir se ficar muito tempo na tela
        if ((timeToDestroy -= Time.fixedDeltaTime) <= 0f)
            Destroy(gameObject);
    }

    public void Initialize(Vector3 _direction, int _byPlayer)
    {
        direction = _direction;
        byPlayer = _byPlayer;
    }

    protected virtual void DoDamage(Collider2D _collider) // dar override no script de projétil do inimigo
    {
        ServerSend.ProjectileDamaged(this);

        if (_collider.CompareTag("Enemy"))
            _collider.GetComponent<GameCharacter>().TakeDamage(damage);

        projectiles.Remove(id);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D _collider = collision.collider;
        DoDamage(_collider); // causa dano se for inimigo
    }
}
