using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private EMovemnet _movement;

    private Animator _animator;
    public Animator Animator => _animator;
    private Transform _target;
    private float _range;
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private Vector3 _prevPosition;
    private float _delta = 0f;

    public event Action OnFire;
    public event Action<Projectile> OnArrive;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(Vector3 startPos, Transform target, float range)
    {
        _startPosition = startPos;
        _target = target;
        _endPosition = target.transform.position;
        _range = range;
    }

    public void Fire()
    {
        if (_movement == EMovemnet.Arc)
            MoveArc().AttachExternalCancellation(destroyCancellationToken).Forget();
        else if (_movement == EMovemnet.Homing)
            MoveHoming();
        OnFire?.Invoke();
    }

    private async UniTask MoveArc()
    {

        while (_delta < 1.0f)
        {
            _delta += Time.deltaTime * _speed;
            _delta = Mathf.Clamp01(_delta);

            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 midPoint = (_startPosition + _target.position) / 2 + Vector3.up * _range * 0.5f;
            Vector3 newPosition = CalculateQuadraticBezierPoint(_delta, _startPosition, midPoint, _target.position);
            transform.position = newPosition;

            Vector3 moveDirection = (transform.position - _prevPosition).normalized;
            if (moveDirection.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = targetRotation * Quaternion.Euler(0, -90, 0); // X축으로 보게 만듬
            }

            _prevPosition = transform.position;
            await UniTask.NextFrame();
        }
        OnArrive?.Invoke(this);
    }

    private void MoveHoming()
    {
        transform.DOMove(_endPosition, _speed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                OnArrive?.Invoke(this);
            });
    }



    // https://youtu.be/Xwj8_z9OrFw?si=x1LzaOykKLDx9fF3
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }


}
