using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyType
{
    melee, 
    ranger, 
    tank, 
    sorcerer
}


public enum EnemyState
{
    chaseStatue,
    chasePlayer,
    attack
}

public class Enemy : GameCharacter
{
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    private static int nextEnemyId = 1;

    public int id;
    public EnemyState state;
    private bool isChasingStatue = false;
    public Player playerTarget;
    public StatueManager statueTarget;
    public Enemy enemyTarget;

    public NavMeshAgent agent;

    public float detectionRange = 10f; // jogador é 0.5
    public float attackRange = 4f;
    private float attackCooldown = 1.5f;
    private float damage = 15f;

    public EnemyType type;

    private float timeStamp = 0f;

    WaitForSeconds destinationDelay = new WaitForSeconds(0.5f);

    private void Start()
    {
        id = nextEnemyId;
        nextEnemyId++;
        enemies.Add(id, this);
        health = maxHealth;

        ServerSend.SpawnEnemy(this);

        state = EnemyState.chaseStatue;
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

    public void Initialize(EnemyType _type) // chamar depois de instanciar
    {
        type = _type;


        switch (type)
        {
            case EnemyType.melee: // default
                break;

            case EnemyType.ranger:
                attackRange = 7.5f;
                attackCooldown = 3f;
                maxHealth = 50f;
                // damage fica no Projectile
                break;

            case EnemyType.sorcerer:
                attackRange = 9.5f;
                attackCooldown = 4f;
                maxHealth = 50f;
                SetMoveSpeed(200f); // 50f mais rápido que jogador
                // damage fica no Projectile
                break;

            case EnemyType.tank:
                attackCooldown = 2f;
                maxHealth = 150f;
                damage = 20f;
                break;

            default:
                break;
        }

        // modifica depois de receber os modificadores acima
        agent.stoppingDistance = attackRange;

    }

    private void FixedUpdate()
    {
        switch (state)
        {
            // procura um jogador. Se não estiver nenhum próximo, segue a estátua.
            case EnemyState.chaseStatue:
                if (!LookForPlayer())
                    ChaseStatue();
                break;
            
            // Se encontrar jogador, vai atrás dele.
            case EnemyState.chasePlayer:
                    ChasePlayer();
                break;

            case EnemyState.attack:
                Attack();
                break;

            default:
                break;
        }

        ServerSend.EnemyPosition(this);
    }

    private bool LookForPlayer()
    {
        // loop por todos os jogadores (se não for sorcerer)
        if (type != EnemyType.sorcerer)
        {
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    // se estiver ao alcance
                    Vector3 _enemyToPlayer = _client.player.transform.position - transform.position;
                    float sqrLenght = _enemyToPlayer.sqrMagnitude;
                    if (sqrLenght <= (detectionRange * detectionRange))
                    {
                        
                        playerTarget = _client.player; // muda o target

                        if (isChasingStatue)
                        {
                            isChasingStatue = false;
                        }

                        // troca pathfinding para o jogador detectado
                        agent.destination = playerTarget.transform.position;

                        state = EnemyState.chasePlayer;
                        Debug.Log("Enemy started chasing player...");

                        return true;
                        
                    }
                }
            }
        }

        else
        {
            // se for Sorcerer:
            // percorrer dicionário de inimigo e encontrar um com vida < maxHealth
            Debug.Log("Enemy is Sorcerer");
            return false;
        }

        return false;
    }

    private void ChaseStatue()
    {
        if (!isChasingStatue)
        {
            StartChasingStatue();
        }

        Vector3 _enemyToStatue = statueTarget.transform.position - transform.position;
        float sqrLenght = _enemyToStatue.sqrMagnitude;

        // se está ao alcance do ataque
        if (sqrLenght <= (attackRange * attackRange))
        {
            state = EnemyState.attack;
        }
    }

    private void StartChasingStatue()
    {
        isChasingStatue = true;

        // ativa pathfinding setando destination para transform da estátua
        agent.destination = statueTarget.transform.position;
    }


    private void ChasePlayer()
    {
        // não seguir jogador morto
        if (playerTarget.health <= 0f) return;


        // Se não está no alcance de ataque, coroutine para SetDestination (checar sorcerer)
        Vector3 _enemyToPlayer = playerTarget.transform.position - transform.position;
        float sqrLenght = _enemyToPlayer.sqrMagnitude;
        if (sqrLenght > (attackRange * attackRange))
        {
            StartCoroutine(SetAIDestination());
        }
        else
            state = EnemyState.attack;
    }

    private void Attack()
    {
        switch (type)
        {
            // mesmo ataque para tank ou melee
            case EnemyType.tank: 
            case EnemyType.melee:
                // se não está em cooldown
                if (timeStamp <= Time.fixedTime)
                {
                    if (isChasingStatue)
                        statueTarget.TakeDamage(damage);
                    else
                        playerTarget.TakeDamage(damage);

                    timeStamp = Time.fixedTime + attackCooldown;
                    state = EnemyState.chaseStatue; // retorna ao estado de procurar pela estátua se tiver longe do jogador
                }                
                break;

            case EnemyType.ranger:
                if (timeStamp <= Time.fixedTime)
                {
                    if (isChasingStatue)
                    {
                        // instanciar projétil Ranger na direção statueTarget
                    }
                    else
                    {
                        // instanciar projétil Ranger na direção playerTarget
                    }

                    timeStamp = Time.fixedTime + attackCooldown;
                    state = EnemyState.chaseStatue;
                }
                break;

            case EnemyType.sorcerer:
                if (timeStamp <= Time.fixedTime)
                {
                    if (isChasingStatue)
                    {
                        // instanciar projétil Ranger na direção statueTarget
                    }
                    else
                    {
                        // instanciar projétil Ranger na direção enemyTarget
                    }

                    timeStamp = Time.fixedTime + attackCooldown;
                    state = EnemyState.chaseStatue;
                }
                break;

            default:
                break;
        }
    }


    private IEnumerator SetAIDestination()
    {
        // seta o destino a cada segundo (delay)
        if (!isChasingStatue)
        {
            agent.destination = playerTarget.transform.position;
            Debug.Log("AI destination set to Player.");
        }
        else
        {
            agent.destination = statueTarget.transform.position;
            Debug.Log("AI destination set to Statue.");
        }

        yield return destinationDelay;
    }


    public override void TakeDamage(float _damage)
    {
        health -= _damage;

        Debug.Log($"Enemy {id} damaged by {_damage}. Current health: {health}.");

        // se nessa execução morreu:
        if (health <= 0f)
        {
            health = 0f;
            enemies.Remove(id);
            Destroy(gameObject);
        }

        ServerSend.EnemyHealth(this);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // se jogador entrou no trigger, ele vira target
        if (collision.gameObject.TryGetComponent<Player>(out Player _whoEntered))
        {
            playerTarget = _whoEntered;
            state = EnemyState.attack;
            isChasingStatue = false;
        }
        else if (collision.gameObject.TryGetComponent<StatueManager>(out StatueManager _statue))
        {
            statueTarget = _statue;
            state = EnemyState.attack;
            isChasingStatue = true;
        }
    }

}
