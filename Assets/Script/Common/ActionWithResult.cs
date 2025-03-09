using System;
using System.Collections.Generic;

public class ActionWithResult
{
    private Dictionary<Func<bool>, Action> _handlers = new Dictionary<Func<bool>, Action>();
    private List<bool> _executionResults = new List<bool>();

    public void AddHandler(Func<bool> handler)
    {
        Action wrappedAction = () =>
        {
            bool result = handler();
            _executionResults.Add(result);
        };

        _handlers[handler] = wrappedAction;
    }

    public void RemoveHandler(Func<bool> handler)
    {
        if (_handlers.ContainsKey(handler))
            _handlers.Remove(handler);
    }
    /// <summary>
    /// 하나라도 true를 리턴한다면, true 리턴.
    /// </summary>
    /// <returns></returns>
    public bool Invoke()
    {
        _executionResults.Clear();

        foreach (var wrappedAction in _handlers.Values)
            wrappedAction?.Invoke();

        foreach (bool result in _executionResults)
            if (result)
                return true;
        return false;
    }

    public void RemoveAllHandlers()
    {
        _handlers.Clear();
        _executionResults.Clear();
    }

    public static ActionWithResult operator +(ActionWithResult actionWithResult, Func<bool> handler)
    {
        actionWithResult.AddHandler(handler);
        return actionWithResult;
    }

    public static ActionWithResult operator -(ActionWithResult actionWithResult, Func<bool> handler)
    {
        actionWithResult.RemoveHandler(handler);
        return actionWithResult;
    }
}