using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(BoxCollider2D))]
public class Monster : Entity
{
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected int _dropCoin;
    [SerializeField] protected int _dropLuckyStone;

    protected int _currentHealth;
    protected bool _canMove = true;
    protected bool _isDeath = false;

    public int MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public int DropCoin => _dropCoin;
    public int DropLuckyStone => _dropLuckyStone;
    public int CurrentHealth => _currentHealth;

    protected List<Transform> _points = new List<Transform>(4);
    protected int _targetPoint = 1;

    protected CancellationTokenSource _cts;

    public event Action<Vector3> OnMove;
    public event Action<float, float> OnHealthChange;

    public event Action<Monster> OnDeath;

    private void OnDisable()
    {
        DisposeCts();
    }

    public void Init(List<Transform> points, int maxHealth)
    {
        _points.AddRange(points);
        _maxHealth = maxHealth;
        ResetMonster();
    }
    public void ResetMonster()
    {
        _currentHealth = _maxHealth;
        _canMove = true;
        _targetPoint = 1;
    }

    public void Move()
    {
        if(_canMove)
            OnMove?.Invoke(_points[_targetPoint % 4].position);
    }

    public async UniTask Stop(float time)
    {
        _canMove = false;
        DOTween.Kill(this); // 이동 멈춤
        DisposeCts();
        _cts = new CancellationTokenSource();
        await UniTask.WaitForSeconds(time).AttachExternalCancellation(_cts.Token); // 사망시 처리
        _canMove = true;
        Move();
    }

    public void TakeDamage(int damage)
    {
        if (_isDeath)
            return;
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        OnHealthChange?.Invoke(_currentHealth, _maxHealth);

        if(_currentHealth <= 0)
            Death();
    }

    public void AddTargetPoint()
    {
        _targetPoint++;
    }

    public virtual void Death()
    {
        _isDeath = true;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    protected void DisposeCts()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
