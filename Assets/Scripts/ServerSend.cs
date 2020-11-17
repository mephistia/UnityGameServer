using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend 
{
    // para 1 cliente
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);

    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    // para todos clientes
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    // para todos clientes exceto um
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
                Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
                Server.clients[i].udp.SendData(_packet);
        }
    }

    // pacote teste

    #region Packets
    public static void Welcome(int _toClient, string _msg)
    {
        // limpar memória após o uso
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }


    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.maxHealth);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);
            SendUDPDataToAll(_packet);
        }
    }


    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectile(Projectile _projectile, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void ProjectileDamaged(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileDamaged))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_projectile.direction);
            _packet.Write(_projectile.collisionTag);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnemy(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);
            _packet.Write((int)_enemy.type);

            SendTCPDataToAll(_packet);
        }
    }

   

    public static void EnemyPosition(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyHealth(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void StatueHealth(StatueManager _statue)
    {
        using (Packet _packet = new Packet((int)ServerPackets.statueHealth))
        {
            _packet.Write(_statue.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnergy(Energy _energy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnergy))
        {
            _packet.Write(_energy.id);
            _packet.Write(_energy.transform.position);
            _packet.Write(_energy.timeToDestroy);

            SendTCPDataToAll(_packet);
        }
    }

    public static void DespawnEnergy(Energy _energy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.despawnEnergy))
        {
            _packet.Write(_energy.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void EnergyPickedUp(Player _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.energyPickedUp))
        {
            _packet.Write(_byPlayer.id);
            // manda apenas para o jogador
            SendTCPData(_byPlayer.id, _packet);
        }
    }

    public static void SpawnProjectileSkill(ProjectileSkill _projectile, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectileSkill))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectileTank(ProjectileTank _projectile, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectileTank))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void TankAttacked(Vector3 _position, bool _attackedEnemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.tankAttacked))
        {
            _packet.Write(_position); // posição para animação
            

            SendTCPDataToAll(_packet);
        }
    }


    #endregion

}
