using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "SO/GameData")]
public class GameDataSO : ScriptableObject
{
    [SerializeField] private int _defaultCoin = 100;
    [SerializeField] private int _defaultLuckyStone = 0;
    [SerializeField] private int _defaultSummonCost = 20;
    [SerializeField] private int _defaultSummonCostIncrease = 2;
    [SerializeField] private int _defaultMaxHeroCount = 20;
    [SerializeField] private int _defaultMaxMonsterCount = 100;
    [SerializeField] private int _defaultWaveClearCoin = 5;
    [SerializeField] private int _defaultWaveAlertTime = 5;

    [Header("운빨 데이터")]
    [SerializeField] private int _defaultRareGachaCost = 1;
    [SerializeField] private int _defaultEpicGachaCost = 2;
    [SerializeField] private float _defaultRareGachaProbability = 0.6f;
    [SerializeField] private float _defaultEpicGachaProbability = 0.2f;

    [Header("강화 데이터")]
    [SerializeField] private int _defaultNormalRareReinforceCost = 30;
    [SerializeField] private int _defaultEpicReinforceCost = 50;
    [SerializeField] private int _defaultMythicReinforceCost = 2;
    [SerializeField] private int _defaultNormalRareReinforceCostIncrease = 30;
    [SerializeField] private int _defaultEpicReinforceCostIncrease = 50;
    [SerializeField] private int _defaultMythicReinforceCostIncrease = 1;

    [Header("판매 데이터")]
    [SerializeField] private int _defaultSellNormal = 12;
    [SerializeField] private int _defaultSellRare = 24;
    [SerializeField] private int _defaultSellEpic = 36;

    // 읽기 전용 프로퍼티
    public int DefaultCoin => _defaultCoin;
    public int DefaultLuckyStone => _defaultLuckyStone;
    public int DefaultSummonCost => _defaultSummonCost;
    public int DefaultSummonCostIncrease => _defaultSummonCostIncrease;
    public int DefaultMaxHeroCount => _defaultMaxHeroCount;
    public int DefaultMaxMonsterCount => _defaultMaxMonsterCount;
    public int DefaultWaveClearCoin => _defaultWaveClearCoin;

    public int DefaultWaveAlertTime => _defaultWaveAlertTime;   

    public int DefaultRareGachaCost => _defaultRareGachaCost;
    public int DefaultEpicGachaCost => _defaultEpicGachaCost;
    public float DefaultRareGachaProbability => _defaultRareGachaProbability;
    public float DefaultEpicGachaProbability => _defaultEpicGachaProbability;

    public int DefaultNormalRareReinforceCost => _defaultNormalRareReinforceCost;
    public int DefaultEpicReinforceCost => _defaultEpicReinforceCost;
    public int DefaultMythicReinforceCost => _defaultMythicReinforceCost;
    public int DefaultNormalRareReinforceCostIncrease => _defaultNormalRareReinforceCostIncrease;
    public int DefaultEpicReinforceCostIncrease => _defaultEpicReinforceCostIncrease;
    public int DefaultMythicReinforceCostIncrease => _defaultMythicReinforceCostIncrease;


    public int DefaultSellNormal => _defaultSellNormal = 12;
    public int DefaultSellRare => _defaultSellRare = 24;
    public int DefaultSellEpic => _defaultSellEpic = 36;
}

