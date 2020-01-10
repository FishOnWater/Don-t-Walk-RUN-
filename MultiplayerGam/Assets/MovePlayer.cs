using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    public TcpClientController TcpClientController;
    public bool Playable;
    private float speed = 7.0f;
    private Vector3 _oldPosition;

    void Start()
    {

    }

    void Update()
    {
        Move();
        Aim();
    }

    void Move()
    {
        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.position += Movement * speed * Time.deltaTime;
    }

    void Aim()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.LookAt(new Vector3(mousePos.x, transform.position.y, mousePos.z));
    }

    void FixedUpdate()
    {
        if (!Playable) return;

        if (transform.position != _oldPosition)
        {
            Message msg = new Message();
            msg.MessageType = MessageType.PlayerMovement;
            PlayerInfo info = new PlayerInfo();
            info.Id = TcpClientController.Player.Id;
            info.Name = TcpClientController.Player.Name;
            info.X = transform.position.x;
            info.Y = transform.position.y;
            info.Z = transform.position.z;
            msg.PlayerInfo = info;
            TcpClientController.SendMessage(msg);
        }

        _oldPosition = transform.position;
    }
}