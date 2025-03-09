using System;

public class WaveModel
{
    public int Wave { get; private set; }
    public int RemainTime { get; private set; }
    public int CurrentCount { get; private set; }

    public event Action<int> OnWaveChanged;
    public event Action<int> OnRemainTimeChanged;
    public event Action<int> OnCurrentCountChanged;

    public WaveModel(int wave, int remainTime, int currentCount)
    {
        Wave = wave;
        RemainTime = remainTime;
        CurrentCount = currentCount;
    }
    public void AddWave()
    {
        Wave++;
        OnWaveChanged?.Invoke(Wave);
    }

    public void ChangeRemainTime(int time)
    {
        RemainTime = time;
        OnRemainTimeChanged?.Invoke(RemainTime);
    }

    public void AddCurrentCount()
    {
        CurrentCount++;
        OnCurrentCountChanged?.Invoke(CurrentCount);
    }

    public void ReduceCurrentCount()
    {
        CurrentCount--;
        OnCurrentCountChanged?.Invoke(CurrentCount);
    }
}

