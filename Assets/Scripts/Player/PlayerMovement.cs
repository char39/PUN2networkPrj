using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 180.0f;
    private PlayerInput playerInput;
    private Rigidbody playerRb;
    private Animator playerAni;

    #region hashValue
    private readonly string hashMove = "Move";
    #endregion

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRb = GetComponent<Rigidbody>();
        playerAni = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
        playerAni.SetFloat(hashMove, playerInput.move);
    }

    private void Move()
    {
        Vector3 moveDistance = moveSpeed * playerInput.move * Time.deltaTime * transform.forward;
        Vector3 newPosition = playerRb.position + moveDistance;
        playerRb.MovePosition(newPosition);
    }
    private void Rotate()
    {
        float turn = rotateSpeed * playerInput.rotate * Time.deltaTime;
        playerRb.rotation = playerRb.rotation * Quaternion.Euler(0f, turn, 0f);
    }

}

