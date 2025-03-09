using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAttackHandler : HeroAttackHandler
{
    [SerializeField] private Projectile _projectile;


    public override void OnAttack()
    {
        Monster target = null;
        foreach(var col in hitColliders)
        {
            if (col == null)
                continue;

            if(col.TryGetComponent<Monster>(out var monster))
                target = monster;
        }

        if (target == null)
            return;

        var projectile = Instantiate(_projectile);
        projectile.transform.position = transform.position;
        projectile.Init(transform.position, target.transform, _attackRange);
        projectile.OnArrive += ((self) =>
        {
            target.TakeDamage(_hero.GetTotalPower());
            Destroy(self.gameObject);
        });
        projectile.Fire();
    }
}
