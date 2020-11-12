using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviors : MonoBehaviour
{
    // Movimentar com inputs do jogador
    public static void MovePlayer(Rigidbody2D _rb2d, Vector2 _movement, float _speed)
    {
        _rb2d.velocity = _movement * _speed;
        //_transform.Translate(_movement * _speed);
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
}
