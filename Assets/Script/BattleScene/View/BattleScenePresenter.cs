using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleScenePresenter : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private HeroDataSO _heroData;
    [SerializeField] private SummonDataSO _summonData;
    [SerializeField] private WaveDataSO _waveData;
    [SerializeField] private float _timeScale  = 1.0f;

    [Header("User Data")]
    [Tooltip("보유한 영웅 목록")]
    [SerializeField] private List<HeroInfo> _ownedHeroList = new List<HeroInfo>();

    [Header("Views")]
    [SerializeField] private BaseView _baseView;
    [SerializeField] private WaveView _waveView;
    [SerializeField] private MythicView _mythicView;
    [SerializeField] private GachaView _gachaView;

    [Header("In game")]
    [SerializeField] private SlotContainer _slotContainer;
    [SerializeField] private MonsterSpawner _mySpawner;
    [SerializeField] private MonsterSpawner _opponentSpawner;

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

    // Start is called before the first frame update
    private void Awake()
    {
        Time.timeScale = _timeScale;
        SetHeroListModel();
        SetResourceModel();
        SetSummonCostModel();
        SetReinforceCostModel();
        SetWaveModel();

        SetSlotContinater();
        SetReinforce();
        SetTimer();
        SetSpawner();
        InitializeViews();

        StartGame();
    }

  
    #region Model
    private void SetHeroListModel()
    {
        _heroListModel = new HeroListModel(_heroData, _ownedHeroList);
        _heroListModel.OnHeroCountChanged += HandleHeroCountChange;
        _heroListModel.OnHeroRemoved += HandleHeroRemove;
    }

    private void SetResourceModel()
    {
        _resourceModel = new ResourceModel(_gameData.DefaultCoin, _gameData.DefaultLuckyStone);
        _resourceModel.OnCoinChanged += HandleCoinChange;
        _resourceModel.OnLuckyStoneChanged += HandleLuckyStoneChange;
    }
    private void SetSummonCostModel()
    {
        _summonModel = new SummonCostModel(_gameData.DefaultSummonCost);
        _summonModel.OnSummonCostChanged += HandleSummonCostChange;
    }

    private void SetReinforceCostModel()
    {
        _reinforceModel = new ReinforceCostModel(_gameData.DefaultNormalRareReinforceCost, _gameData.DefaultEpicReinforceCost, _gameData.DefaultMythicReinforceCost);
        _reinforceModel.OnNormalRareReinforceCostChanged += HandleNormalRareReinforceCostChange;
        _reinforceModel.OnEpicReinforceCostChanged += HandleEpicReinforceCostChange;
        _reinforceModel.OnMythicReinforceCostChanged += HandleMythicReinforceCostChange;
    }

    private void SetWaveModel()
    {
        _waveModel = new WaveModel(1, _gameData.DefaultWaveAlertTime, 0);
        _waveModel.OnWaveChanged += HandleWaveChange;
        _waveModel.OnRemainTimeChanged += HandleRemainTimeChange;
        _waveModel.OnCurrentCountChanged += HandleCurrentCountChange;
    }

    #endregion


    private void SetTimer()
    {
        _timer = new Timer(_gameData.DefaultWaveAlertTime);
        _timer.OnTick += HandleTimerTick;
        _timer.OnTimeZero += HandleTimerZero;
    }

    private void SetSpawner()
    {
        _mySpawner.Init(_resourceModel, _waveModel);
        _opponentSpawner.Init(_resourceModel,_waveModel);

        _mySpawner.OnSpawn += HandleMonsterSpawn;
        _opponentSpawner.OnSpawn += HandleMonsterSpawn;
    }
    private void SetSlotContinater()
    {
        _slotContainer.OnSellHero += HandleSellHero;
        _slotContainer.OnCombineHero += HandleCombineHero;

    }

    private void StartGame()
    {
        _timer.Start(this.destroyCancellationToken);
    }
    private void LoseGame()
    {
        Debug.Log("실패");
        _mySpawner.SetOver();
        _opponentSpawner.SetOver();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
        _timer.Stop();
    }

    private void WinGame()
    {
        Debug.Log("클리어!");
        _timer.Stop();
    }
    private void SetReinforce()
    {
        _normalRareReinforce = new Reinforce();
        _epicReinforce = new Reinforce();
        _mythicReinforce = new Reinforce();
        _summonReinforce = new Reinforce();
    }

    private void InitializeViews()
    {
        if (_baseView != null)
        {
            _baseView.Init(_gameData.DefaultSummonCost);
            _baseView.UpdateMaxHeroCount(GetMaxHeroCount());
            _baseView.UpdateColor(_resourceModel.Coin >= _summonModel.SummonCost);
            _baseView.UpdateCost(_summonModel.SummonCost);
            _baseView.UpdateCurrentCoin(_resourceModel.Coin);
            _baseView.UpdateCurrentLuckyStone(_resourceModel.LuckyStone);
            _baseView.UpdateCurrentHeroCount(_heroListModel.SummonedHeroCount);
            _baseView.OnSummonClicked += Summon;
            _baseView.OnMythicClicked += PopupCombine;
            _baseView.OnUnbbalClicked += PopupGacha;
            _baseView.OnReinforceClicked += PopupReinforce;
        }

        if(_waveView != null)
        {
            _waveView.UpdateWaveNumber(_waveModel.Wave);
            _waveView.UpdateRemainTime(_waveModel.RemainTime);
            _waveView.UpdateCurrentCount(_waveModel.CurrentCount);
            _waveView.UpdateMaxCount(GetMaxMonsterCount());
        }

        if(_mythicView != null)
        {
            _mythicView.Init(_heroListModel);
            _mythicView.OnMakeClick += AddHero;
        }

        if(_gachaView != null)
        {
            _gachaView.UpdateLuckyStone(_resourceModel.LuckyStone);
            _gachaView.UpdateCurrentHeroCount(_heroListModel.SummonedHeroCount);
            _gachaView.UpdateMaxHero(GetMaxHeroCount());
            _gachaView.OnRareGachaClick += GachaRare;
            _gachaView.OnEpicGachaClick += GachaEpic;
        }
    }

    private void Summon()
    {
        if (_heroListModel.SummonedHeroCount >= GetMaxHeroCount())
        {
            Debug.Log("인구수 최대");
            return;
        }

        if (!_slotContainer.IsEmptySlotExist())
        {
            Debug.Log("소환 공간 없음");
            return;
        }

        int summonCost = _summonModel.SummonCost;

        if (_resourceModel.Coin < summonCost)
        {
            Debug.Log("코인 부족");
            return;
        }

        Hero hero = GetRandomHero();

        if(hero == null)
        {
            Debug.LogError("영웅 정보가 없습니다.");
            return;
        }

        _resourceModel.ReduceCoin(summonCost);
        _summonModel.AddSummonCost(_gameData.DefaultSummonCostIncrease);

        AddHero(hero);
    }

    private void AddHero(Hero hero)
    {
        if(hero is MythicHero mythic)
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

    private void GachaRare()
    {
        if (_heroListModel.SummonedHeroCount >= GetMaxHeroCount())
            return;

        _resourceModel.ReduceLuckyStone(1);

        float value = UnityEngine.Random.Range(0, 1.0f);
        if(value <= _gameData.DefaultRareGachaProbability)
            AddHero(GetRandomHero(EGrade.Rare));
    }

    private void GachaEpic()
    {
        if (_heroListModel.SummonedHeroCount >= GetMaxHeroCount())
            return;

        _resourceModel.ReduceLuckyStone(1);

        float value = UnityEngine.Random.Range(0, 1.0f);
        if (value <= _gameData.DefaultEpicGachaProbability)
            AddHero(GetRandomHero(EGrade.Epic));
    }


    private void PopupCombine()
    {
        _mythicView.Show();
    }
    private void PopupGacha()
    {
        _gachaView.Show();
    }

    private void PopupReinforce()
    {

    }



    #region Handler

    private void HandleSummonCostChange(int cost)
    {
        _baseView?.UpdateCost(cost);
    }

    private void HandleHeroCountChange(int count)
    {
        _baseView?.UpdateCurrentHeroCount(count);
        _gachaView?.UpdateCurrentHeroCount(count);
    }

    private void HandleHeroRemove(Hero hero)
    {
        _slotContainer.RemoveHero(hero);
    }

    private void HandleSellHero(Hero hero)
    {
        switch(hero.Grade)
        {
            case EGrade.Normal:
                _resourceModel.AddCoin(_gameData.DefaultSellNormal);
                break;
            case EGrade.Rare:
                _resourceModel.AddCoin(_gameData.DefaultSellRare);
                break;
            case EGrade.Epic:
                _resourceModel.AddCoin(_gameData.DefaultSellEpic);
                break;
        }

        _heroListModel.RemoveSummonHero(hero);
        
    }
    private void HandleCombineHero(List<Hero> heroes)
    {
        EGrade targetGrade;
        switch (heroes.First().Grade)
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
                return;
        }
        foreach (Hero hero in heroes)
            _heroListModel.RemoveSummonHero(hero);

        AddHero(GetRandomHero(targetGrade));
    }

    private void HandleCoinChange(int currentCoin)
    {
        _baseView?.UpdateCurrentCoin(currentCoin);
        _baseView?.UpdateColor(currentCoin >= _summonModel.SummonCost);
    }
    private void HandleLuckyStoneChange(int currentLuckyStone)
    {
        _baseView?.UpdateCurrentLuckyStone(currentLuckyStone);
        _gachaView?.UpdateLuckyStone(currentLuckyStone);
    }
    private void HandleNormalRareReinforceCostChange(int cost)
    {

    }

    private void HandleEpicReinforceCostChange(int cost)
    {

    }

    private void HandleMythicReinforceCostChange(int cost)
    {

    }

    private void HandleWaveChange(int wave)
    {
        _waveView?.UpdateWaveNumber(wave);
    }

    private void HandleRemainTimeChange(int time)
    {
        _waveView?.UpdateRemainTime(time);
    }
    private void HandleCurrentCountChange(int count)
    {
        _waveView?.UpdateCurrentCount(count);
        _waveView?.UpdateGuage(count, GetMaxMonsterCount());
        if (count >= GetMaxMonsterCount())
            LoseGame();
    }

    private void HandleMonsterSpawn()
    {
        _waveModel.AddCurrentCount();
    }

    private void HandleTimerTick(int value)
    {
        _waveModel.ChangeRemainTime(value);
    }

    private void HandleTimerZero()
    {
        _waveModel.AddWave();

        if(!_waveData.TryGetWaveInfo(_waveModel.Wave, out var waveInfo))
        {
            WinGame();
            return;
        }

        if (_mySpawner.BossMonsterCount > 0 || _opponentSpawner.BossMonsterCount > 0)
        {
            LoseGame();
            return;
        }
        _timer.SetTime(waveInfo.Time);
        _mySpawner.Spawn(waveInfo);
        _opponentSpawner.Spawn(waveInfo);
    }

    #endregion
    
    
    
    // 예시용. 최대 보유 영웅수를 반환합니다.
    // 유물의 효과 등으로 최대 영웅 보유수가 증가 할 경우 여기서 추가처리 하여 리턴하면 됩니다.
    private int GetMaxHeroCount()
    {
        return _gameData.DefaultMaxHeroCount;
    }

    private int GetMaxMonsterCount()
    {
        return _gameData.DefaultMaxMonsterCount;
    }
}

