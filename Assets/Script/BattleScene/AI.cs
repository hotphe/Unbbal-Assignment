using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private HeroDataSO _heroData;
    [SerializeField] private SummonDataSO _summonData;
    [SerializeField] private WaveDataSO _waveData;

    [Header("Hero Data")]
    [Tooltip("보유한 영웅 목록")]
    [SerializeField] private List<HeroInfo> _ownedHeroList = new List<HeroInfo>();

    [Header("In game")]
    [SerializeField] private AISlotContainer _slotContainer;
    [SerializeField] private MonsterSpawner _mySpawner;
    [SerializeField] private MonsterSpawner _opponentSpawner;

    [SerializeField] private float _aiOperateInterval = 0.3f;

    private HeroListModel _heroListModel;
    private ResourceModel _resourceModel;
    private SummonCostModel _summonModel;
    private ReinforceCostModel _reinforceModel;
    private WaveModel _waveModel;

    private Timer _timer;

    private Reinforce _normalRareReinforce;
    private Reinforce _epicReinforce;
    private Reinforce _mythicReinforce;
    private Reinforce _summonReinforce;
    private void Awake()
    {
        SetHeroListModel();
        SetResourceModel();
        SetSummonCostModel();
        SetReinforceCostModel();
        SetWaveModel();
        SetReinforce();
        SetSpawner();

        Operate().AttachExternalCancellation(destroyCancellationToken).Forget();
    }

    private async UniTask Operate()
    {
        while(true)
        {
            await UniTask.WaitForSeconds(_aiOperateInterval);
            TrySummon();
            await UniTask.WaitForSeconds(_aiOperateInterval);
            TryMakeMythic();
            await UniTask.WaitForSeconds(_aiOperateInterval);
            TryCombine();
        }
    }
    private void TrySummon()
    {
        if (_resourceModel.Coin < _summonModel.SummonCost)
            return;

        if (_heroListModel.SummonedHeroCount >= GetMaxHeroCount())
            return;

        if (!_slotContainer.IsEmptySlotExist())
            return;

        _resourceModel.ReduceCoin(_summonModel.SummonCost);
        _summonModel.AddSummonCost(_gameData.DefaultSummonCostIncrease);

        Hero hero = GetRandomHero();
        AddHero(hero);
    }

    private void AddHero(Hero hero)
    {
        if (hero is MythicHero mythic)
        {
            var requires = mythic.CombineToList();
            foreach (var id in requires)
                _heroListModel.RemoveSummonHeroById(id);
        }

        var instantiated = _slotContainer.SummonHero(hero);
        switch (instantiated.Grade)
        {
            case EGrade.Normal:
            case EGrade.Rare:
                instantiated.SetReinforce(_normalRareReinforce);
                break;
            case EGrade.Epic:
                instantiated.SetReinforce(_epicReinforce);
                break;
            case EGrade.Mythic:
                instantiated.SetReinforce(_mythicReinforce);
                break;
        }

        _heroListModel.AddSummonHero(instantiated);
        instantiated.gameObject.SetActive(true);

    }
    private Hero GetRandomHero(EGrade grade)
    {
        HeroInfo heroInfo = _heroListModel.GetRandomHeroInfo(grade);

        if (!_heroData.TryGetHero(heroInfo.HeroId, out var hero))
            return null;
        return hero;
    }

    private Hero GetRandomHero()
    {
        int level = Mathf.Clamp(_summonReinforce.Level, 0, _summonData.Count - 1);

        if (!_summonData.TryGetSummonInfo(level, out var info))
        {
            Debug.LogError($"Invalid summonReinforce level ({level})");
            return null;
        }

        float total = info._normalRate + info._rareRate + info._epicRate;
        float value = UnityEngine.Random.Range(0, total);

        HeroInfo heroInfo = null;

        if (value < info._normalRate)
            heroInfo = _heroListModel.GetRandomHeroInfo(EGrade.Normal);
        else if (value < info._normalRate + info._rareRate)
            heroInfo = _heroListModel.GetRandomHeroInfo(EGrade.Rare);
        else
            heroInfo = _heroListModel.GetRandomHeroInfo(EGrade.Epic);

        if (heroInfo == null)
            return null;

        if (!_heroData.TryGetHero(heroInfo.HeroId, out var hero))
            return null;
        return hero;
    }

    private void TryMakeMythic()
    {
        foreach(var info in _heroListModel.MythicHeroes)
        {
            if (!_heroData.TryGetHero(info.HeroId, out var hero))
                continue;

            if (hero is not MythicHero mythic)
                continue;

            if(CheckRequires(mythic))
                AddHero(mythic);
        }
    }

    private bool CheckRequires(MythicHero mythic)
    {
        List<int> requires = mythic.CombineToList();
        List<int> owned = _heroListModel.SummonedHeroes.Select(x=>x.Id).ToList();
        Dictionary<int, int> ownedCount = new Dictionary<int, int>();

        foreach (var id in owned)
        {
            if (!ownedCount.ContainsKey(id))
                ownedCount[id] = 0;
            ownedCount[id]++;
        }

        bool canMake = true;
        for (int i = 0; i < requires.Count; i++)
        {
            int id = requires[i];

            if (ownedCount.ContainsKey(id) && ownedCount[id] > 0)
                ownedCount[id]--;
            else
                canMake = false;
        }
        return canMake;
    }

    private void TryCombine()
    {
        foreach(var slot in _slotContainer.Slots)
        {
            if (slot.Count != 3)
                continue;

            EGrade targetGrade;
            switch (slot.Hero.Grade)
            {
                case EGrade.Normal:
                    targetGrade = EGrade.Rare;
                    break;
                case EGrade.Rare:
                    targetGrade = EGrade.Epic;
                    break;
                case EGrade.Epic:
                case EGrade.Mythic:
                default:
                    continue;
            }

            foreach (var hero in slot.GetHeroUnits().ToList())
                slot.RemoveHero(hero);
            AddHero(GetRandomHero(targetGrade));
        }
    }

    private void SetReinforce()
    {
        _normalRareReinforce = new Reinforce();
        _epicReinforce = new Reinforce();
        _mythicReinforce = new Reinforce();
        _summonReinforce = new Reinforce();
    }
    private void SetSpawner()
    {
        _mySpawner.Init(_resourceModel, _waveModel);
        _opponentSpawner.Init(_resourceModel, _waveModel);
    }

    #region Model
    private void SetHeroListModel()
    {
        _heroListModel = new HeroListModel(_heroData, _ownedHeroList);
        _heroListModel.OnHeroRemoved += HandleHeroRemove;
    }

    private void HandleHeroRemove(Hero hero)
    {
        _slotContainer.RemoveHero(hero);
    }

    private void SetResourceModel()
    {
        _resourceModel = new ResourceModel(_gameData.DefaultCoin, _gameData.DefaultLuckyStone);
    }
    private void SetSummonCostModel()
    {
        _summonModel = new SummonCostModel(_gameData.DefaultSummonCost);
    }

    private void SetReinforceCostModel()
    {
        _reinforceModel = new ReinforceCostModel(_gameData.DefaultNormalRareReinforceCost, _gameData.DefaultEpicReinforceCost, _gameData.DefaultMythicReinforceCost);
    }

    private void SetWaveModel()
    {
        _waveModel = new WaveModel(1, _gameData.DefaultWaveAlertTime, 0);
    }

    #endregion

    private int GetMaxHeroCount()
    {
        return _gameData.DefaultMaxHeroCount;
    }

    private int GetMaxMonsterCount()
    {
        return _gameData.DefaultMaxMonsterCount;
    }
}
