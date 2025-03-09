using System;

public class ResourceModel
{
    public int Coin { get; private set; }
    public int LuckyStone { get; private set; }

    public event Action<int> OnCoinChanged;
    public event Action<int> OnLuckyStoneChanged;

    public ResourceModel(int coin, int luckyStone)
    {
        Coin = coin;
        LuckyStone = luckyStone;
    }

    public void AddCoin(int value) 
    { 
        Coin += value; 
        OnCoinChanged?.Invoke(Coin);
    }
    public void ReduceCoin(int value) 
    { 
        Coin -= value;
        OnCoinChanged?.Invoke(Coin);
    }

    public void AddLuckyStone(int value) 
    {  
        LuckyStone += value; 
        OnLuckyStoneChanged?.Invoke(LuckyStone);
    }
    public void ReduceLuckyStone(int value) 
    {  
        LuckyStone -= value;
        OnLuckyStoneChanged?.Invoke(LuckyStone);
    }
}

