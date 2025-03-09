using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AISlotContainer : MonoBehaviour
{
    [SerializeField] private List<Slot> _slots = new List<Slot>(18);
    public IList<Slot> Slots => _slots;
    public Hero SummonHero(Hero hero)
    {
        Slot targetSlot;
        var heroExistSlots = _slots.Where(x => x.Count > 0 && x.Count < 3)
            .Where(x => x.Hero.Id == hero.Id);

        if (heroExistSlots.Any())
            targetSlot = heroExistSlots.First();
        else
            targetSlot = GetEmptySlot();

        Hero newHero = Instantiate(hero);
        newHero.gameObject.SetActive(false);
        targetSlot.AddHero(newHero);

        return newHero;
    }
    public void RemoveHero(Hero hero)
    {
        foreach (var slot in _slots)
        {
            if (!slot.GetHeroUnits().Contains(hero))
                continue;
            slot.RemoveHero(hero);
            return;
        }
    }
    public bool IsEmptySlotExist()
    {
        return _slots.Where(x => x.Count == 0).Count() > 0;
    }
    private Slot GetEmptySlot()
    {
        return _slots.Where(x => x.Count == 0).First();
    }
}
