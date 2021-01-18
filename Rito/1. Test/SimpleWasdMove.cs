using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18
// 작성자 : Rito

public class SimpleWasdMove : MonoBehaviour
{
    public Animator Anim { get; private set; }
    public Vector3 _moveVector;

    [Header("Options")]
    [Range(1f, 20f)]
    public float _moveSpeed = 3f;

    [Range(1f, 5f)]
    public float _runSpeedMultiplier = 1.5f;

    [Range(1f, 10f)]
    public float _turnSpeed = 4f;
    public string _animMoveParam = "Move Speed";
    public KeyCode _runKey = KeyCode.LeftShift;

    private float _animSpeed = 0f;

    private void Start()
    {
        Anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Make Move Vector
        float xMove = 0f;
        float zMove = 0f;
        if (Input.GetKey(KeyCode.W)) zMove += 1f;
        if (Input.GetKey(KeyCode.S)) zMove -= 1f;
        if (Input.GetKey(KeyCode.D)) xMove += 1f;
        if (Input.GetKey(KeyCode.A)) xMove -= 1f;

        _moveVector = new Vector3(xMove, 0f, zMove).normalized;
        _animSpeed = Mathf.RoundToInt(_moveVector.magnitude);

        // Run ?
        if (Input.GetKey(_runKey))
        {
            _moveVector *= _runSpeedMultiplier;
            _animSpeed *= 2f;
        }

        // Move, Rotate
        if(_animSpeed > 0)
        {
            transform.position += (_moveVector * _moveSpeed * Time.deltaTime);

            var rotDir = Quaternion.LookRotation(_moveVector);
            float prevYRot = transform.eulerAngles.y;
            float nextYRot = rotDir.eulerAngles.y;

            float rotBoost = Mathf.Abs(nextYRot - prevYRot) / 90f;

            var nextRot = Quaternion.RotateTowards(transform.rotation, rotDir, _turnSpeed * Time.deltaTime * 100f * rotBoost);

            transform.rotation = nextRot;
        }

        // Update Animator
        if(Anim != null)
            Anim.SetFloat(_animMoveParam, _animSpeed);
    }
}