using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GachaView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    [SerializeField] private Button _rareGachaBtn;
    [SerializeField] private Button _epicGachaBtn;
    [SerializeField] private Button _exitBtn;

    [SerializeField] private TextMeshProUGUI _luckyStoneCount;
    [SerializeField] private TextMeshProUGUI _currentHeroCount;
    [SerializeField] private TextMeshProUGUI _maxHeroCount;



    public event Action OnRareGachaClick;
    public event Action OnEpicGachaClick;

    private void Awake()
    {
        _exitBtn.onClick.AddListener(Hide);
        _rareGachaBtn.onClick.AddListener(()=>OnRareGachaClick?.Invoke());
        _epicGachaBtn.onClick.AddListener(() => OnEpicGachaClick?.Invoke());
    }
    public void Show()
    {
        _panel.SetActive(true);
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }
    public void UpdateLuckyStone(int value)
    {
        _luckyStoneCount.SetText(value.ToString());
    }

    public void UpdateCurrentHeroCount(int value)
    {
        _currentHeroCount.SetText(value.ToString());
    }

    public void UpdateMaxHero(int value)
    {
        _maxHeroCount.SetText(value.ToString());
    }
}
