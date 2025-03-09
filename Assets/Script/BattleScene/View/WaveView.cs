using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveNumber;
    [SerializeField] private TextMeshProUGUI _remainTime;
    [SerializeField] private TextMeshProUGUI _currentCount;
    [SerializeField] private TextMeshProUGUI _maxCount;
    [SerializeField] private Image _monsteGuage;

    public void UpdateWaveNumber(int value)
    {
        _waveNumber.SetText(value.ToString());
    }

    public void UpdateRemainTime(int value)
    {
        _remainTime.SetText(value.ToString("D2"));
    }

    public void UpdateCurrentCount(int value)
    {
        _currentCount.SetText(value.ToString());
    }
    public void UpdateMaxCount(int value)
    {
        _maxCount.SetText(value.ToString());
    }

    public void UpdateGuage(int current, int max)
    {
        _monsteGuage.fillAmount = current / (float)max;
    }

}
