using PCS.Common;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
public class MythicView : MonoBehaviour
{
    [SerializeField] private HeroDataSO _heroData;
    [SerializeField] private MythicButton _mythicBtn;

    [SerializeField] private Button _makeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Image _mythicImage;

    [SerializeField] private GameObject _mythicCombinePanel;

    private List<MythicButton> _mythicBtns = new List<MythicButton>();

    [SerializeField] private SerializedDictionary<int, Sprite> _idSpriteHeroPair = new SerializedDictionary<int, Sprite>();

    [SerializeField] private GameObject _unionPanel;
    [SerializeField] private List<Image> _unionHeroImage = new List<Image>();
    [SerializeField] private List<Image> _unionHeroGradeImage = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _unionHeroHasText = new List<TextMeshProUGUI>();
    [SerializeField] private Image _targetUnionHeroImage;

    private MythicHero _makable;
    private HeroListModel _heroListModel;

    public event Action<Hero> OnMakeClick;

    private void Awake()
    {
        _exitButton.onClick.AddListener(() => Hide());
    }

    public void Init(HeroListModel model)
    {
        _heroListModel = model;
        foreach (var hero in _heroListModel.MythicHeroes)
        {
            var btn = Instantiate(_mythicBtn, _mythicBtn.transform.parent);

            if (!_idSpriteHeroPair.TryGetValue(hero.HeroId, out Sprite sprite))
                continue;
            btn.SetHeroId(hero.HeroId);
            btn.ChangeImage(sprite);
            btn.gameObject.SetActive(true);
            btn.OnButtonClick += ShowUnion;
            _mythicBtns.Add(btn);
        }
    }

    public void Show()
    {
        _mythicCombinePanel.gameObject.SetActive(true);
        if(_mythicBtns.Count > 0)
            ShowUnion(_mythicBtns[0].Id);
    }


    public void ShowUnion(int id)
    {
        if (!_heroData.TryGetHero(id, out var hero))
            return;

        if (hero is not MythicHero mythic)
            return;

        _makeButton.onClick.RemoveAllListeners();
        if (CheckRequires(mythic))
        {
            _makeButton.gameObject.SetActive(true);
            _makeButton.onClick.AddListener(() =>
            {
                Hide();
                OnMakeClick?.Invoke(mythic);
            });
        }
        else
        {
            _makeButton.gameObject.SetActive(false);
        }
    }

    private bool CheckRequires(MythicHero mythic)
    {
        List<int> requires = mythic.CombineToList();
        List<int> owned = _heroListModel.SummonedHeroes.Select(x => x.Id).ToList();
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

            if (!_heroData.TryGetHero(id, out var hero))
                continue;

            if (!_idSpriteHeroPair.TryGetValue(id, out Sprite sprite))
                continue;

            _unionHeroImage[i].sprite = sprite;

            switch(hero.Grade)
            {
                case EGrade.Normal:
                    _unionHeroGradeImage[i].color = Color.white;
                    break;
                case EGrade.Rare:
                    _unionHeroGradeImage[i].color = Color.blue;
                    break;
                case EGrade.Epic:
                    _unionHeroGradeImage[i].color = Color.magenta;
                    break;
            }
            if (ownedCount.ContainsKey(id) && ownedCount[id] > 0)
            {
                _unionHeroHasText[i].SetText("보유중");
                ownedCount[id]--;
            }
            else
            {
                _unionHeroHasText[i].SetText("미보유");
                canMake = false;
            }
        }

        if (!_idSpriteHeroPair.TryGetValue(mythic.Id, out Sprite mythicSprite))
            return false;
        _targetUnionHeroImage.sprite = mythicSprite;
        return canMake;
    }


    private void Hide()
    {
        _mythicCombinePanel.gameObject.SetActive(false);
    }







}
