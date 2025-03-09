using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

[RequireComponent(typeof(InputHandler))]
public class SlotContainer : MonoBehaviour
{
    [SerializeField] private HeroControllerView _controllerView;
    [SerializeField] private Transform _summonPoint;
    [SerializeField] private SummonTrail _summonTrail;

    [SerializeField] private MoveLineRenderer _lineRenderer;

    [SerializeField] private SpriteRenderer _selectMark;
    [SerializeField] private SpriteRenderer _targetMark;
    [SerializeField] private SpriteRenderer _rangeSprite;

    [SerializeField] private List<Slot> _slots =new List<Slot>(18);

    public event Action<Hero> OnSellHero;
    public event Action<List<Hero>> OnCombineHero;

    private void Awake()
    {
        _slots.AddRange(GetComponentsInChildren<Slot>());

        var handler = GetComponent<InputHandler>();

        handler.OnSelectSlot += ShowRange;
        handler.OnClickSlot += ShowControlPanel;
        handler.OnDragStart += ShowSlots;
        handler.OnDragingSlot += ShowMoveMark;
        handler.OnEndDragSlot += MoveHero;
        handler.OnClickEmpty += HideViews;

        _controllerView.OnSellClicked += TrySellHero;
        _controllerView.OnCombineClicked += TryCombineHero;
    }

    public bool IsEmptySlotExist()
    {
        return _slots.Where(x => x.Count == 0).Count() > 0;
    }

    public Hero SummonHero(Hero hero)
    {
        Slot targetSlot;
        var heroExistSlots = _slots.Where(x => x.Count > 0 && x.Count < 3)
            .Where(x => x.Hero.Id == hero.Id);

        if(heroExistSlots.Any())
            targetSlot = heroExistSlots.First();
        else
            targetSlot = GetEmptySlot();

        Hero newHero = Instantiate(hero);
        newHero.gameObject.SetActive(false);
        targetSlot.AddHero(newHero);
        
        var trail = CreateTrail(newHero.Grade, _summonPoint.position, targetSlot.transform.position);
        trail.Play(()=>newHero.gameObject.SetActive(true));
        return newHero;
    }

    public void RemoveHero(Hero hero)
    {
        foreach(var slot in _slots)
        {
            if (!slot.GetHeroUnits().Contains(hero))
                continue;
            slot.RemoveHero(hero);
            return;
        }
    }

    private Slot GetEmptySlot()
    {
        return _slots.Where(x=>x.Count == 0).First();
    }

    private SummonTrail CreateTrail(EGrade grade, Vector3 startPosition, Vector3 targetPosition)
    {
        var trail = Instantiate(_summonTrail, startPosition, Quaternion.identity);

        trail.Init(grade, startPosition, targetPosition);
        return trail;
    }


    private void TrySellHero(Slot slot)
    {
        OnSellHero?.Invoke(slot.GetHeroUnits().Last());
        HideViews();
    }

    private void TryCombineHero(Slot slot)
    {
        if (slot.Count != 3)
            return;
        
        OnCombineHero?.Invoke(slot.GetHeroUnits().ToList());
        HideViews();
    }

    private void ShowSlots(Slot origin)
    {
        foreach(var slot in _slots)
            slot.gameObject.SetActive(true);
        _selectMark.enabled = true;
        _selectMark.transform.position = origin.transform.position;
    }

    private void HideSlots()
    {
        foreach (var slot in _slots)
        {
            if (slot.Count > 0)
                continue;
            slot.gameObject.SetActive(false);
        }
    }
    private void ShowRange(Slot origin)
    {
        _rangeSprite.enabled = true;
        _rangeSprite.transform.position = origin.transform.position;
        _rangeSprite.transform.localScale = Vector3.one * origin.Hero.AttackRange;
    }

    private void ShowMoveMark(Slot origin, Slot target)
    {
        _targetMark.enabled = true;
        _targetMark.transform.position = target.transform.position;
        _lineRenderer.gameObject.SetActive(true);
        _lineRenderer.SetPosition(origin.transform.localPosition, target.transform.localPosition);
    }

    private void MoveHero(Slot origin, Slot target)
    {
        origin.ExchangeHero(target);
        HideViews();
    }

    private void ShowControlPanel(HeroSlot target)
    {
        if (target.Hero.Grade == EGrade.Mythic)
            return;
        HideSlots();
        _selectMark.enabled = false;
        _controllerView.SetSlot(target);
        _controllerView.Show();
    }
    
    private void HideViews()
    {
        _controllerView.Hide();
        HideSlots();
        _selectMark.enabled = false;
        _targetMark.enabled = false;
        _rangeSprite.enabled = false;
        _lineRenderer.gameObject.SetActive(false);
    }

}
