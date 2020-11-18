using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    // reconhecer o cliente conectado
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIDCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");

        if (_fromClient != _clientIDCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIDCheck})!");

        }

        // colocar jogador no jogo
        Server.clients[_fromClient].SendIntoGame(_username);

    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        Server.clients[_fromClient].player.SetInput(_inputs);
    }

    public static void PlayerRotation(int _fromClient, Packet _packet)
    {
        float _angle = _packet.ReadFloat();

        Server.clients[_fromClient].player.SetAngle(_angle);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void PlayerShootSkill(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ShootSkill(_shootDirection);
    }

    public static void PlayerTankSkill(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();
        float _pressedTime = _packet.ReadFloat();

        Server.clients[_fromClient].player.ShootSkill(_shootDirection, _pressedTime);
    }

    public static void AskCombine(int _fromClient, Packet _packet)
    {
        int _playerID = _packet.ReadInt();

        if (_playerID == _fromClient)
            Server.clients[_fromClient].player.WaitCombine(true);
    }

}
