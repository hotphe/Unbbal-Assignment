using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private int _id;
    [SerializeField] private int _level;
    [SerializeField] private EGrade _grade;
    [SerializeField] private float _power;
    [Tooltip("레벨 당 공격력 증가량")]
    [SerializeField] private float _powerIncrease;
    [Tooltip("초당 공격 횟수")]
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _moveSpeed;
    private bool _isMoving = false;
    private Reinforce _reinforce;
    private Animator _animator;
    private Tween _moveTween;

    private ActionWithResult _onAttack = new ActionWithResult();

    public int Id => _id;
    public int Level => _level;
    public EGrade Grade => _grade;
    public float Power => _power;
    public float PowerIncrease => _powerIncrease;
    public float AttackSpeed => _attackSpeed;
    public float AttackRange => _attackRange;
    public float MoveSpeed => _moveSpeed;
    public bool IsMoving => _isMoving;
    public ActionWithResult OnAttack => _onAttack;

    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (IsMoving) // 이동중이면 공격 불가
            return;

        if (TryAttack()) // 하나라도 공격 행동 실행 시, idle 애니메이션 실행하지 않음.
            return;

        SetIdle();
    }

    public void MoveHero(Vector3 position)
    {
        _isMoving = true;
        _animator.Play("Move");
        _moveTween?.Kill();
        if (position.x > transform.position.x) //오른쪽으로 이동시 뒤집기
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = Vector3.one;

        _moveTween = transform.DOMove(position, _moveSpeed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                transform.localScale = Vector3.one;
                _animator.Play("Idle");
            });
    }
    /// <summary>
    /// 공격하면 true, 공격하지 않으면 false
    /// </summary>
    /// <returns></returns>
    private bool TryAttack()
    {
        if (_isMoving)
            return false;
        return _onAttack.Invoke();
    }

    private void SetIdle()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // Idle 상태가 아니면
        { 
            _animator.speed = 1;
            _animator.CrossFade("Idle",0);
        }
    }

    public void SetReinforce(Reinforce reinforce)
    {
        _reinforce = reinforce;
    }

    public int GetTotalPower()
    {
        float power = _power;
        power += _level * _powerIncrease; // 레벨 당 공격력 증가량
        power *= 1 + (_reinforce.Level * 0.5f); // 해당 게임 등급별 강화량

        return Mathf.CeilToInt(power);
    }

}
