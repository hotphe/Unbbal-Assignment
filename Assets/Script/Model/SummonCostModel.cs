using System;

public class SummonCostModel
{
    public int SummonCost { get; private set; }
    public event Action<int> OnSummonCostChanged;

    public SummonCostModel(int summonCost)
    {
        SummonCost = summonCost;
    }

    public void AddSummonCost(int value)
    {
        SummonCost += value;
        OnSummonCostChanged?.Invoke(SummonCost);
    }
}

