using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameCharacter
{
    public int id;
    public string username;
   
    public int maxLifes = 3;
    public int currentLifes;

    private bool[] inputs;
    private float angle = 0f;

    public int maxEnergies = 5;

    public PolygonCollider2D tankTrigger;
    public List<Collider2D> stuffOnTrigger = new List<Collider2D>(); // itens adicionados no objeto de trigger

    private float areaDamage = 0;

    private void Start()
    {
        SetMoveSpeed(GetMoveSpeed() * Time.fixedDeltaTime);
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        currentLifes = maxLifes;
        inputs = new bool[4];
        velocity = Vector2.zero;

        // mudar collider
        if (id == 2)
        {
            GetComponent<CircleCollider2D>().enabled = false;
            areaDamage = 12f;
        }
        else
        {
            GetComponent<PolygonCollider2D>().enabled = false;
            tankTrigger.enabled = false;
        }
    }

    public void FixedUpdate()
    {
        if (health <= 0f)
            return;


        Vector2 _inputDirection = Vector2.zero;

        if (inputs[0])
        {
            _inputDirection.y += 1;
        }

        if (inputs[1])
        {
            _inputDirection.x -= 1;
        }

        if (inputs[2])
        {
            _inputDirection.y -= 1;
        }

        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
        Rotate(angle);
    }

    private void Move(Vector2 _inputDirection)
    {
        
        // Aplica no rigidbody2d do jogador
        GameBehaviors.MovePlayer(rb2d, _inputDirection, GetMoveSpeed());

        // responder com o id recebido
        ServerSend.PlayerPosition(this);
    }

    private void Rotate(float _angle)
    {
        GameBehaviors.RotateToAngle(this.transform, _angle);

        // responder com o id recebido
        ServerSend.PlayerRotation(this);
    }

    public void Shoot(Vector3 _facing)
    {
        if (health <= 0)
            return;

        if (id == 1)
            GameBehaviors.PlayerShoot(shootOrigin, _facing, id);
        else
        {
            // saber quais inimigos atacou
            bool[] attackedEnemy = new bool[stuffOnTrigger.Count];
            foreach (Collider2D col in stuffOnTrigger)
                attackedEnemy[stuffOnTrigger.IndexOf(col)] = GameBehaviors.PlayerAreaAttack(_facing, col, areaDamage);

            // feedback visual
            bool atLeastOneEnemy = false;
            foreach (bool _attacked in attackedEnemy)
            {
                if (_attacked) atLeastOneEnemy = true;
                break;
            }
            Debug.Log($"Attacked enemies? {atLeastOneEnemy}");

            Vector3 animPos = tankTrigger.transform.position;

            ServerSend.TankAttacked(animPos, atLeastOneEnemy);
        } 
    }

    public void ShootSkill(Vector3 _facing, float _pressedTime = 0f)
    {
        if (health <= 0)
            return;

        if (id == 1)
            GameBehaviors.PlayerShootSkill(shootOrigin, _facing, id);
        else
        {
            float strengthPercent = Mathf.InverseLerp(0, 2, _pressedTime);

            GameBehaviors.PlayerTankSkill(shootOrigin, _facing, id, strengthPercent);

            // projéteis enviam info para os clientes em seus próprios scripts
        }
    }

    public override void TakeDamage(float _damage)
    {
        if (health <= 0f)
            return;

        health -= _damage;

        if (health <= 0f)
        {
            health = 0f;
            currentLifes--;

            if (currentLifes > 0)
            {
                // respawn
                Vector3 _position = (id == 1) ? new Vector3(-6.5f, 0, 0) : new Vector3(6.5f, 0, 0);
                transform.position = _position;
                ServerSend.PlayerPosition(this);
                StartCoroutine(Respawn());
            }
            else
            {
                // GameOver? Assistir o outro jogador?
                Debug.Log("Game Over");
            }
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }

    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    public void SetAngle(float _angle)
    {
        angle = _angle;
    }

}
