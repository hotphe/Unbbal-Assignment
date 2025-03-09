using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Button _mythicBtn;
    [SerializeField] private Button _summonBtn;
    [SerializeField] private Button _unbbalBtn;
    [SerializeField] private Button _reinforceBtn;

    [SerializeField] private TextMeshProUGUI _currentCoinText;
    [SerializeField] private TextMeshProUGUI _currentLuckyStoneText;
    [SerializeField] private TextMeshProUGUI _currentHeroText;
    [SerializeField] private TextMeshProUGUI _maxHeroText;

    [SerializeField] private TextMeshProUGUI _summonCostText;

    public event Action OnMythicClicked;
    public event Action OnSummonClicked;
    public event Action OnUnbbalClicked;
    public event Action OnReinforceClicked;

    private void Start()
    {
        _mythicBtn.onClick.AddListener(() => OnMythicClicked?.Invoke());
        _summonBtn.onClick.AddListener(() => OnSummonClicked?.Invoke());
        _unbbalBtn.onClick.AddListener(() => OnUnbbalClicked?.Invoke());
        _reinforceBtn.onClick.AddListener(() => OnReinforceClicked?.Invoke());
    }

    public void Init(int cost)
    {
        _summonCostText.SetText(cost.ToString());
    }

    public void UpdateColor(bool canSummon)
    {
        if (canSummon)
            _summonCostText.color = Color.white;
        else
            _summonCostText.color = Color.red;
    }

    public void UpdateCost(int cost)
    {
        _summonCostText.SetText(cost.ToString());
    }

    public void UpdateCurrentCoin(int value)
    {
        _currentCoinText.SetText(value.ToString());
    }

    public void UpdateCurrentLuckyStone(int value)
    {
        _currentLuckyStoneText.SetText(value.ToString());
    }

    public void UpdateCurrentHeroCount(int value)
    {
        _currentHeroText.SetText(value.ToString());
    }    

    public void UpdateMaxHeroCount(int value)
    {
        _maxHeroText.SetText(value.ToString());
    }
}
