using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviors : MonoBehaviour
{
    // Movimentar com inputs do jogador
    public static void MovePlayer(Rigidbody2D _rb2d, Vector2 _movement, float _speed)
    {
        _rb2d.velocity = _movement * _speed;
    }


    // Rotacionar para um ângulo
    public static void RotateToAngle(Transform _transform, float _angle)
    {
        //_transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_angle));
        _transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_angle));
    }

    // Atirar
    public static void PlayerShoot(Transform _shootOrigin, Vector3 _facing, int _playerId)
    {
        NetworkManager.instance.InstantiateProjectile(_shootOrigin).Initialize(_facing, _playerId);
    }

    public static void PlayerShootSkill(Transform _shootOrigin, Vector3 _facing, int _playerId)
    {
        NetworkManager.instance.InstantiateProjectileSkill(_shootOrigin).Initialize(_facing, _playerId);
    }

    public static bool PlayerAreaAttack(Vector3 _facing, Collider2D _col, float _damage)
    {
       if (_col.TryGetComponent<Enemy>(out Enemy _enemy))
        {
            _enemy.TakeDamage(_damage); // já envia vida do inimigo
            return true;
        }

        return false;
    }

    public static void PlayerTankSkill(Transform _shootOrigin, Vector3 _facing, int _playerId, float _strength)
    {
        NetworkManager.instance.InstantiateProjectileTank(_shootOrigin).Initialize(_facing, _playerId, _strength);
    }

    public static void CombinePlayers(Player _playerRanger, Player _playerTank)
    {
        // desativar collider do ranger
        _playerRanger.GetComponent<CircleCollider2D>().enabled = false;

        _playerRanger.transform.position = _playerTank.transform.position;
        ServerSend.PlayerPosition(_playerRanger);

    }

    public static void UndoCombine(Player _playerRanger, Player _playerTank)
    {
        _playerRanger.GetComponent<CircleCollider2D>().enabled = true;

        _playerRanger.isCombined = _playerTank.isCombined = false;


    }
}
