using PCS.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "SO/HeroData")]
public class HeroDataSO : ScriptableObject
{
    [SerializeField] private SerializedDictionary<int, Hero> _allHeroes = new SerializedDictionary<int, Hero>(); // ���̵�, ����� ������
    public bool TryGetHero(int id, out Hero hero)
    {
        if (_allHeroes.TryGetValue(id, out hero))
            return true;
        return false;
    }

    public bool Contains(int id)
    {
        return _allHeroes.ContainsKey(id);
    }
}
