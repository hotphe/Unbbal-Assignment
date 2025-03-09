using PCS.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MythicHero : Hero
{
    [SerializeField] private SerializedDictionary<int, int> _combineDictionary = new SerializedDictionary<int, int>(); // 아이디, 필요갯수
    public IReadOnlyDictionary<int, int> CombineDictionary => _combineDictionary;

    public bool CanCombine(IList<Hero> heroes)
    {
        foreach(var id in _combineDictionary.Keys)
        {
            int count = heroes.Where(x => x.Id == id).Count();
            if (count < _combineDictionary[id])
                return false;
        }
        return true;
    }

    public List<int> CombineToList()
    {
        List<int> list = new List<int>();  
        foreach(var id in _combineDictionary.Keys)
            list.Add(id);
        return list;
    }


}
