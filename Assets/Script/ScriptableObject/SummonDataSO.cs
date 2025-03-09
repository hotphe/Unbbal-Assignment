using PCS.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonData", menuName = "SO/SummonData")]
public class SummonDataSO : ScriptableObject
{
    [SerializeField] private SerializedDictionary<int, SummonInfo> _summonInfos = new SerializedDictionary<int, SummonInfo>(); // 강화 레벨, 소환 확률

    public int Count => _summonInfos.Count;
    public bool TryGetSummonInfo(int level, out SummonInfo info)
    {
        if (_summonInfos.TryGetValue(level, out info))
            return true;
        return false;
    }
}
