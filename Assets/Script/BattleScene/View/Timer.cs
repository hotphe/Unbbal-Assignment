using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class Timer
{ 
    public int Time { get; private set; }

    public bool _isOver = false;
    public event Action<int> OnTick;
    public event Action OnTimeZero;

    public Timer(int time)
    {
        Time = time;
    }

    public void SetTime(int time)
    {
        Time = time -1; // 기본 20초지만 0초에서 1초가 걸림 따라서 19초가 세팅되게함
    }

    public void Stop()
    {
        _isOver = true;
    }

    public void Start(CancellationToken ct)
    {
        Run().AttachExternalCancellation(ct).Forget();
    }

    public async UniTask Run()
    {
        while(!_isOver)
        {
            await UniTask.WaitForSeconds(1);
            Time--;
            if (Time < 0)
                OnTimeZero?.Invoke();
            OnTick?.Invoke(Time);
        }
    }
}

