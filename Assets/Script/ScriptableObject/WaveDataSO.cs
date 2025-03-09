using PCS.Common;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveData", menuName = "SO/WaveData")]
public class WaveDataSO : ScriptableObject
{
    //웨이브 , 몬스터정보
    [SerializeField] private SerializedDictionary<int, WaveInfo> _waves = new SerializedDictionary<int, WaveInfo>();

    public bool TryGetWaveInfo(int wave, out WaveInfo waveInfo)
    {
        if (_waves.TryGetValue(wave, out waveInfo))
            return true;
        return false;
    }
}
