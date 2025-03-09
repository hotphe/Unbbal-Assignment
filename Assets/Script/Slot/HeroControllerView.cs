using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroControllerView : MonoBehaviour
{
    [SerializeField] private RectTransform _controlPanel;
    [SerializeField] private Button _sellBtn;
    [SerializeField] private Button _combineBtn;

    private HeroSlot _slot;

    private Canvas _canvas;

    public event Action<HeroSlot> OnSellClicked;
    public event Action<HeroSlot> OnCombineClicked;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        _sellBtn.onClick.AddListener(()=> OnSellClicked?.Invoke(_slot));
        _combineBtn.onClick.AddListener( ()=> OnCombineClicked?.Invoke(_slot));
    }

    public void SetSlot(HeroSlot slot)
    {
        _slot = slot;
        _controlPanel.position = slot.transform.position;
        if(slot.Hero.Grade == EGrade.Epic)
            _combineBtn.gameObject.SetActive(false);
        else
            _combineBtn.gameObject.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
