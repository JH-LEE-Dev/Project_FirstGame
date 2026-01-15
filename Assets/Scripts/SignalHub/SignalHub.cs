using System;
using System.Collections.Generic;

public class SignalHub
{
    // 인터페이스를 통해 서로 다른 제네릭 리스트를 하나의 딕셔너리에 담습니다.
    private interface ISignalHandlerList { }

    private class SignalHandlerList<T> : ISignalHandlerList
    {
        public readonly List<Action<T>> Handlers = new(16);
    }

    // 인스턴스마다 별도의 저장소를 가집니다.
    private readonly Dictionary<Type, ISignalHandlerList> _storage = new();

    public void Subscribe<T>(Action<T> handler)
    {
        var list = GetOrCreateList<T>();
        if (!list.Handlers.Contains(handler))
        {
            list.Handlers.Add(handler);
        }
    }

    public void UnSubscribe<T>(Action<T> handler)
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            var list = (SignalHandlerList<T>)listObj;
            list.Handlers.Remove(handler);
        }
    }

    public void Publish<T>(T signal)
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            var list = (SignalHandlerList<T>)listObj;
            var handlers = list.Handlers;

            // 역순 순회로 복사본 생성(GC 할당) 방지
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                handlers[i]?.Invoke(signal);
            }
        }
    }

    private SignalHandlerList<T> GetOrCreateList<T>()
    {
        Type type = typeof(T);
        if (!_storage.TryGetValue(type, out var list))
        {
            list = new SignalHandlerList<T>();
            _storage[type] = list;
        }
        return (SignalHandlerList<T>)list;
    }

    // 인스턴스가 파괴될 때 모든 구독 정보를 날려버리는 기능
    public void Clear()
    {
        _storage.Clear();
    }
}