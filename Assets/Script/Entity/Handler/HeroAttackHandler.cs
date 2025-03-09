using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Hero))]
public abstract class HeroAttackHandler : MonoBehaviour
{
    [Tooltip("최대 몇명까지 공격 가능한가")]
    [SerializeField] protected int _attackCount;
    [SerializeField] protected LayerMask _targetLayer;

    [SerializeField] private string _animationName;

    protected Animator _animator;
    protected Hero _hero;

    protected Collider2D[] hitColliders;

    protected float _attackRange;
    protected float _attackSpeed;
    protected bool _isAnimating;

    protected void Awake()
    {
        _hero = GetComponent<Hero>();
        _animator = GetComponentInChildren<Animator>();
        _attackSpeed = _hero.AttackSpeed;
        _attackRange = _hero.AttackRange;
    }

    protected void OnEnable()
    {
        _hero?.OnAttack.AddHandler(HandleAttack);
    }

    protected void OnDisable()
    {
        _hero?.OnAttack.RemoveHandler(HandleAttack);
    }

    protected bool HandleAttack()
    {
        if (_isAnimating)
            return true;
        hitColliders = new Collider2D[_attackCount];
        int colCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _attackRange * 0.5f * transform.lossyScale.x,
            hitColliders,
            _targetLayer
            );

        if (colCount == 0)
            return false;

        _animator.speed = _attackSpeed;
        _animator.CrossFade(_animationName, 0, 0, 0);
        WaitAnimationEnd().AttachExternalCancellation(destroyCancellationToken).Forget();
        return true;
    }

    public async UniTask WaitAnimationEnd()
    {
        _isAnimating = true;
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await UniTask.Yield();
        }
        _isAnimating = false;
    }

    public abstract void OnAttack();
}
