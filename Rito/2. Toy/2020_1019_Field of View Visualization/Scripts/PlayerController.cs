using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera _viewCam;

    [Range(1f, 10f)]
    public float _speed = 6f;

    private Rigidbody _rb;

    private float _h;
    private float _v;
    private Vector3 _velocity;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Vector3 mousePos = _viewCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _viewCam.transform.position.y));
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");

        transform.LookAt(mousePos);
        _velocity = new Vector3(_h, 0f, _v);

        _rb.MovePosition(_rb.position + _velocity * _speed * 0.05f);
    }

    private void Init()
    {
        TryGetComponent(out _rb);

        if (_viewCam == null)
            _viewCam = Camera.main;
    }
}
