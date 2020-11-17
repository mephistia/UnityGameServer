using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    // waves
    public EnemyType[][] waves = 
    { 
        // wave [0]
        new EnemyType[] 
        { 
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee,
            EnemyType.melee
        }, 

        // wave [1]
        new EnemyType[]
        {
             EnemyType.melee,
             EnemyType.ranger,
             EnemyType.melee,
             EnemyType.melee,
             EnemyType.ranger,
             EnemyType.melee,
             EnemyType.melee,
             EnemyType.ranger,
             EnemyType.melee,
             EnemyType.melee,
             EnemyType.ranger,
             EnemyType.melee
        }
    
    };

    public float cooldown = 7f;
    public float timeStamp = 0f;
    public int currentWave = 0; // id
    public Transform spawnerLeft, spawnerRight;
    public Vector3 currentSpawner;
    public bool spawnedLeft = false;
    private int currentID = 0;

    private void Start()
    {
        Debug.Log($"Wave 1: {waves[currentWave].Length} enemies.");
        Debug.Log($"Wave 2: {waves[currentWave + 1].Length} enemies.");
    }

    private void FixedUpdate()
    {
        if (Server.clients.TryGetValue(2, out Client _client))
        {
            if (_client.player)
            {
                // se não está em cooldown e o contador de inimigo existe na wave atual
                if (timeStamp <= Time.fixedTime && currentID < waves[currentWave].Length)
                {
                    // alternar entre spawner da esquerda e da direita
                    currentSpawner = (spawnedLeft) ? spawnerRight.position : spawnerLeft.position;

                    Enemy newEnemy = NetworkManager.instance.InstantiateEnemy(currentSpawner);
                    newEnemy.Initialize(waves[currentWave][currentID]);

                    if (currentSpawner == spawnerLeft.position)
                        spawnedLeft = true;
                    else
                        spawnedLeft = false;

                    currentID++;
                    timeStamp = Time.fixedTime + cooldown;
                }

                // ativar próxima wave:
                //else if (currentID >= waves[currentWave].Lenght)
                //{
                //  currentWave++;
                //  currentID = 0;
                //}
            }
            else 
                Debug.Log("Waiting for players...");
        }
        else
            Debug.Log("Waiting for players...");

    }

}
