using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public string userName;

    public Vector3 position;
    public Quaternion rotation;

    List<Users> users = new List<Users>();

    //private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] inputs;

    public Player(int id, string userName, Vector3 spawnPosition)
    {
        this.id = id;
        this.userName = userName;
        this.position = spawnPosition;
        this.rotation = Quaternion.identity;

        inputs = new bool[4];
    }

    public void Update()
    {

    }

    private void Move(Vector2 inputDirection)
    {
        //Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
        //Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

        //Vector3 _moveDirection = right * inputDirection.X + forward * inputDirection.Y;
        //position += _moveDirection * moveSpeed;

        //ServerSend.PlayerPosition(this, users);
        //ServerSend.PlayerRotation(this, users);
    }

    public void SetInput(Vector3 position, Quaternion rotation, List<Users> users)
    {
        this.position = position;
        this.rotation = rotation;
        this.users = users;
    }

}