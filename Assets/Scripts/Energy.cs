using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public static Dictionary<int, Energy> energies = new Dictionary<int, Energy>();
    private static int nextEnergyId = 1;

    public int id;
    public float timeToDestroy = 7f;
    public float characterHealingValue = 15f;
    public float statueHealingValue = 10f;

    private void Start()
    {
        id = nextEnergyId;
        nextEnergyId++;
        energies.Add(id,this);

        ServerSend.SpawnEnergy(this);
    }

    private void FixedUpdate()
    {
        if ((timeToDestroy -= Time.fixedDeltaTime) <= 0f)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<Player>(out Player _player))
        {
            if (_player.energyCount < _player.maxEnergies)
            {
                _player.energyCount++;
                Debug.Log($"Energy sent to player {_player.id}");
                ServerSend.EnergyPickedUp(_player);
                Despawn();
            }

        }
        else if (col.TryGetComponent<Enemy>(out Enemy _enemy))
        {
            _enemy.energyCount++;
            Despawn();
        }
    }


    private void Despawn()
    {
        ServerSend.DespawnEnergy(this);
        energies.Remove(id);
        Destroy(gameObject);
    }
}
