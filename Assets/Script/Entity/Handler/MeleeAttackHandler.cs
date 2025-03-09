
public class MeleeAttackHandler : HeroAttackHandler
{

    public override void OnAttack()
    {
        foreach(var col in hitColliders)
        {
            if (col == null)
                continue;

            if (col.TryGetComponent<Monster>(out var monster))
                monster.TakeDamage(_hero.GetTotalPower());
        }
    }
}
