using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackHandler : HeroAttackHandler
{
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _explosionRange;
    public override void OnAttack()
    {
        Monster target = null;
        foreach (var col in hitColliders)
        {
            if (col == null)
                continue;

            if (col.TryGetComponent<Monster>(out var monster))
                target = monster;
        }

        if (target == null)
            return;

        var projectile = Instantiate(_projectile);
        projectile.transform.position = transform.position;
        projectile.Init(transform.position, target.transform, _attackRange);
        projectile.OnArrive += DamageRange;
        projectile.Fire();
    }

    private void DamageRange(Projectile self)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            self.transform.position,
            _explosionRange,
            _targetLayer);

        if (self.Animator != null)
        {
            self.Animator.CrossFade("Explosion", 0, 0, 0);
            self.transform.localScale = Vector3.one * 3;
        }
        
        foreach(var col in colliders)
        {
            if (!col.transform.TryGetComponent<Monster>(out var monster))
                continue;
            monster.TakeDamage(_hero.GetTotalPower());
        }
        WaitAnimationEnd(self.Animator).AttachExternalCancellation(self.destroyCancellationToken).Forget();
    }

    public async UniTask WaitAnimationEnd(Animator animator)
    {
        await UniTask.NextFrame();
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            await UniTask.Yield();
        Destroy(animator.gameObject);
    }
}
