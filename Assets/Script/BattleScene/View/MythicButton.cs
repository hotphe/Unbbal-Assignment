using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MythicButton : MonoBehaviour
{
    [SerializeField] private Image _heroImage;
    private Button _button;
    private int _id;
    public int Id => _id;
    public event Action<int> OnButtonClick;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnButtonClick?.Invoke(_id));
    }

    public void SetHeroId(int id)
    {
        _id = id;
    }

    public void ChangeImage(Sprite sprite)
    {
        _heroImage.sprite = sprite;
    }
}
