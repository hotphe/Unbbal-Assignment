using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Monster))] // 몬스터에게만 체력이 있음 
public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthbar;
    [SerializeField] private float _scale;
    [SerializeField] private float _height;
    private SpriteRenderer _renderer;
    private Monster _monster;    
    private void Awake()
    {
        _monster = GetComponent<Monster>();

        var healthbar = Instantiate(_healthbar, transform);
        healthbar.transform.localPosition = Vector3.up * _height;
        healthbar.transform.localScale = Vector3.one * _scale;
        healthbar.gameObject.SetActive(false);
        _renderer = healthbar.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (_monster != null)
        {
            _monster.OnHealthChange += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        if (_monster != null)
            _monster.OnHealthChange -= UpdateHealthBar;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if(!_renderer.gameObject.activeSelf)
            _renderer.gameObject.SetActive(true);
        _renderer.material.SetFloat("_HealthRatio", currentHealth/ maxHealth);
    }
    
}
