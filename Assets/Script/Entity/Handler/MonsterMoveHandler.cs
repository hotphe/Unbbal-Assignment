using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Monster))]
public class MonsterMoveHandler : MonoBehaviour
{
    [SerializeField] private Transform _unitRoot;
    private Animator _animator;
    private Monster _monster;

    private float _moveSpeed;
    private Tween _moveTween;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _animator = GetComponentInChildren<Animator>();
        _moveSpeed = _monster.MoveSpeed;
    }

    private void OnEnable()
    {
        if (_monster != null)
            _monster.OnMove += HandleMove;
    }
    private void OnDisable()
    {
        _moveTween?.Kill();
        if( _monster != null )
            _monster.OnMove -= HandleMove;
    }

    private void HandleMove(Vector3 targetPos)
    {
        _animator.Play("Move");
        if (targetPos.x > transform.position.x)
            _unitRoot.localScale = new Vector3(-1, 1, 1);
        else
            _unitRoot.localScale = Vector3.one;

        _moveTween = transform.DOMove(targetPos, _moveSpeed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .SetId(1)
            .OnComplete(() =>
            {
                _monster.AddTargetPoint();
                _monster.Move();
            });
    }


}
