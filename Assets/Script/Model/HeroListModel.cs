using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroListModel
{
    // Linq 사용을 줄이기 위해 등급별 캐싱
    public readonly List<HeroInfo> NormalHeroes = new List<HeroInfo>();
    public readonly List<HeroInfo> RareHeroes = new List<HeroInfo>();
    public readonly List<HeroInfo> EpicHeroes = new List<HeroInfo>();
    public readonly List<HeroInfo> MythicHeroes = new List<HeroInfo>();

    public readonly List<Hero> SummonedHeroes = new List<Hero>();

    public int SummonedHeroCount => SummonedHeroes.Count;

    public event Action<Hero> OnHeroRemoved;
    public event Action<int> OnHeroCountChanged;

    public HeroListModel(HeroDataSO heroData, IEnumerable<HeroInfo> heroList)
    {
        foreach (var info in heroList)
        {
            if (!heroData.TryGetHero(info.HeroId, out var hero))
                continue;

            switch (hero.Grade)
            {
                case EGrade.Normal:
                    NormalHeroes.Add(info);
                    break;
                case EGrade.Rare:
                    RareHeroes.Add(info);
                    break;
                case EGrade.Epic:
                    EpicHeroes.Add(info);
                    break;
                case EGrade.Mythic:
                    MythicHeroes.Add(info);
                    break;
            }
        }
    }


    public HeroInfo GetRandomHeroInfo(EGrade grade)
    {
        int value = -1;
        switch(grade)
        {
            case EGrade.Normal:
                if (NormalHeroes.Count == 0) // 최소 하나는 보장되지만 에러 방지용
                    return null;
                value = UnityEngine.Random.Range(0, NormalHeroes.Count);
                return NormalHeroes[value];
            case EGrade.Rare:
                if (RareHeroes.Count == 0)
                    return null;
                value = UnityEngine.Random.Range(0, RareHeroes.Count);
                return RareHeroes[value];
            case EGrade.Epic:
                if (EpicHeroes.Count == 0)
                    return null;
                value = UnityEngine.Random.Range(0, EpicHeroes.Count);
                return EpicHeroes[value];
        }
        return null;
    }


    public void AddSummonHero(Hero hero)
    {
        SummonedHeroes.Add(hero);
        OnHeroCountChanged?.Invoke(SummonedHeroes.Count);
    }
    public void RemoveSummonHero(Hero hero)
    {
        SummonedHeroes.Remove(hero);
        OnHeroRemoved?.Invoke(hero);
        OnHeroCountChanged?.Invoke(SummonedHeroes.Count);
    }

    public void RemoveSummonHeroById(int id)
    {
        var hero = SummonedHeroes.Find(x=> x.Id == id);
        RemoveSummonHero(hero);
    }
}

