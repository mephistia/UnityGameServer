using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Vector2 velocity;

    public Rigidbody2D rb2d;

    private float moveSpeed = 60f;
    private bool[] inputs;
    private float angle;

    private void Start()
    {
        moveSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        inputs = new bool[4];
        velocity = Vector2.zero;
        angle = 0;
    }

    public void FixedUpdate()
    {
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
        GameBehaviors.MovePlayer(rb2d, _inputDirection, moveSpeed);

        //ServerSend.PlayerVelocity(this);
        ServerSend.PlayerPosition(this);
    }

    private void Rotate(float _angle)
    {
        GameBehaviors.RotateToAngle(this.transform, _angle);

        ServerSend.PlayerRotation(this);
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
