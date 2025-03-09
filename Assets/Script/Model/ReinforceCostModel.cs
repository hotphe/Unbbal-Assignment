using System;

public class ReinforceCostModel
{
    public int NormalRareReinforceCost { get; private set; }
    public int EpicReinforceCost { get; private set; }
    public int MythicReinforceCost { get; private set; }

    public event Action<int> OnNormalRareReinforceCostChanged;
    public event Action<int> OnEpicReinforceCostChanged;
    public event Action<int> OnMythicReinforceCostChanged;

    public ReinforceCostModel(int normalRareReinforceCost, int epicReinforceCost, int mythicReinforceCost)
    {
        NormalRareReinforceCost = normalRareReinforceCost;
        EpicReinforceCost = epicReinforceCost;
        MythicReinforceCost = mythicReinforceCost;
    }

    public void AddNormalRareReinforceCost(int value)
    {
        NormalRareReinforceCost += value;
        OnNormalRareReinforceCostChanged?.Invoke(NormalRareReinforceCost);
    }

    public void AddEpicReinforceCost(int value)
    {
        EpicReinforceCost += value;
        OnEpicReinforceCostChanged?.Invoke(EpicReinforceCost);
    }

    public void AddMythicReinforceCost(int value)
    {
        MythicReinforceCost += value;
        OnMythicReinforceCostChanged?.Invoke(MythicReinforceCost);
    }
}

