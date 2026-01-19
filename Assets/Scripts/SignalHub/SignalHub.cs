using CardSystemSignals;
using System;
using System.Collections.Generic;
using static SignalHub;

/*public class SignalHub
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
}*/

public interface ISignalSubscriber
{
    public void Subscribe<T>(Action<T> handler) where T : struct;

    public void UnSubscribe<T>(Action<T> handler) where T : struct;

    public void Subscribe<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct;

    public void UnSubscribe<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct;
}

// 스코프 신호 구조체 (공용 어셈블리에 위치)
public struct ScopeSignal<T> where T : struct
{
    public bool IsBegin;
    public T Context;
}

public class SignalHub : ISignalSubscriber
{
    private interface ISignalHandlerList { }
    private class SignalHandlerList<T> : ISignalHandlerList
    {
        public readonly List<Action<T>> Handlers = new(16);
    }

    public delegate void SpanHandler<TContext, TData>(TContext context, ReadOnlySpan<TData> data);
    private class SpanSignalHandlerList<TContext, TData> : ISignalHandlerList
    {
        public readonly List<SpanHandler<TContext, TData>> Handlers = new(16);
    }

    private readonly Dictionary<Type, ISignalHandlerList> _storage = new();
    private readonly Dictionary<(Type, Type), ISignalHandlerList> _spanStorage = new();

    // --- [기본 구독 및 발행] ---
    public void Subscribe<T>(Action<T> handler) where T : struct
    {
        var list = GetOrCreateList<T>();
        if (!list.Handlers.Contains(handler)) list.Handlers.Add(handler);
    }

    public void UnSubscribe<T>(Action<T> handler) where T : struct
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
            ((SignalHandlerList<T>)listObj).Handlers.Remove(handler);
    }

    public void Publish<T>(T signal) where T : struct
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            var handlers = ((SignalHandlerList<T>)listObj).Handlers;
            for (int i = handlers.Count - 1; i >= 0; i--) handlers[i]?.Invoke(signal);
        }
    }

    // --- [Span 지원] ---
    public void Subscribe<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct
    {
        var list = GetOrCreateSpanList<TContext, TData>();
        if (!list.Handlers.Contains(handler)) list.Handlers.Add(handler);
    }

    public void UnSubscribe<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct
    {
        var key = (typeof(TContext), typeof(TData));
        if (_spanStorage.TryGetValue(key, out var listObj))
            ((SpanSignalHandlerList<TContext, TData>)listObj).Handlers.Remove(handler);
    }

    public void Publish<TContext, TData>(TContext context, ReadOnlySpan<TData> data) where TContext : struct
    {
        var key = (typeof(TContext), typeof(TData));
        if (_spanStorage.TryGetValue(key, out var listObj))
        {
            var handlers = ((SpanSignalHandlerList<TContext, TData>)listObj).Handlers;
            for (int i = handlers.Count - 1; i >= 0; i--) handlers[i]?.Invoke(context, data);
        }
    }

    // --- [Scope 지원] ---
    // 시작과 끝을 알리는 헬퍼 메서드
    public void BeginScope<T>(T context) where T : struct
    {
        Publish(new ScopeSignal<T> { IsBegin = true, Context = context });
    }

    public void EndScope<T>(T context) where T : struct
    {
        Publish(new ScopeSignal<T> { IsBegin = false, Context = context });
    }

    // Scope 전용 구독 (별도 메서드로 분리하여 명확성 유지)
    public void SubscribeScope<T>(Action<ScopeSignal<T>> handler) where T : struct
    {
        Subscribe(handler);
    }

    public void UnSubscribeScope<T>(Action<ScopeSignal<T>> handler) where T : struct
    {
        UnSubscribe(handler);
    }

    // --- [내부 유틸리티] ---
    private SignalHandlerList<T> GetOrCreateList<T>() where T : struct
    {
        var type = typeof(T);
        if (!_storage.TryGetValue(type, out var list))
            _storage[type] = list = new SignalHandlerList<T>();
        return (SignalHandlerList<T>)list;
    }

    private SpanSignalHandlerList<TContext, TData> GetOrCreateSpanList<TContext, TData>() where TContext : struct
    {
        var key = (typeof(TContext), typeof(TData));
        if (!_spanStorage.TryGetValue(key, out var list))
            _spanStorage[key] = list = new SpanSignalHandlerList<TContext, TData>();
        return (SpanSignalHandlerList<TContext, TData>)list;
    }
}