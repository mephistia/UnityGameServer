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
        _transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_angle));
    }

    // Atirar
    //public static void Shoot(GameObject _projectile, Vector3 _direction, Transform _fromTransform)
    //{

    //    GameObject proj = GameObject.Instantiate(_projectile, _fromTransform.position, Quaternion.identity);
    //    proj.GetComponent<Projectile>()._direction = _direction;
    //    Debug.Log($"Projétil lançado na direção {_direction}");
    //}
}
