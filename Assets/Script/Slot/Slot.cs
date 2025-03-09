using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SlotPosition _oneSlotPosition;
    [SerializeField] private SlotPosition _twoSlotPosition;
    [SerializeField] private SlotPosition _threeSlotPosition;
    private Hero _hero;
    private List<Hero> _heroUnits = new List<Hero>(3);

    public Hero Hero => _hero;
    public int Count => _heroUnits.Count;

    private void SetHeroPosition()
    {
        switch (Count)
        {
            case 1:
                _oneSlotPosition.SetHero(_heroUnits);
                break;
            case 2:
                _twoSlotPosition.SetHero(_heroUnits);
                break;
            case 3:
                _threeSlotPosition.SetHero(_heroUnits);
                break;
        }
    }

    public void ExchangeHero(Slot slot)
    {
        List<Hero> heroes = new List<Hero>();
        heroes.AddRange(_heroUnits);
        RemoveAll(); // 내 슬롯 비움

        MoveHeroes(slot._heroUnits); // 타겟 슬롯 히어로들 추가
        slot.RemoveAll(); // 타겟 슬롯 히어로 비움
        slot.MoveHeroes(heroes);

        ExecuteMoveAnim();
        slot.ExecuteMoveAnim();
    }

    public void ExecuteMoveAnim()
    {
        IList<Transform> transforms = GetPositionTransforms();

        for (int i = 0; i < Count; i++)
        {
            _heroUnits[i].MoveHero(transforms[i].position);
            _heroUnits[i].transform.parent = transforms[i].transform;
        }
    }

    private IList<Transform> GetPositionTransforms()
    {
        switch (Count)
        {
            case 1:
                return _oneSlotPosition.GetTransforms();
            case 2:
                return _twoSlotPosition.GetTransforms();
            case 3:
                return _threeSlotPosition.GetTransforms();
        }
        return null;
    }

    public void AddHero(Hero hero)
    {
        if (_hero == null)
        {
            _hero = hero;
            gameObject.SetActive(true);
        }
        _heroUnits.Add(hero);
        SetHeroPosition();
    }

    public void MoveHeroes(IEnumerable<Hero> heroes)
    {
        if (heroes.Count() == 0) // 올 영웅들이 없으면
        {
            _hero = null;
            return;
        }

        _hero = heroes.First();
        gameObject.SetActive(true);

        foreach (var hero in heroes)
        {
            _heroUnits.Add(hero);
        }
    }

    public IEnumerable<Hero> GetHeroUnits()
    {
        return _heroUnits;
    }

    public void RemoveHero(Hero hero)
    {
        _heroUnits.Remove(hero);
        Destroy(hero.gameObject);

        if (_heroUnits.Count == 0)
            _hero = null;
    }

    public void RemoveAll()
    {
        _heroUnits.Clear();
        _hero = null;
    }

}
