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

public struct ScopeSignal<T>
{
    public bool IsBegin;
    public T Context;
}

// 1. 신호 분류를 위한 마커
public interface ISignalMarker { }
public interface IPulicSignal : ISignalMarker { } 
public interface ICardSystemPrivateSignal : ISignalMarker { }

// 2. 제네릭 구독 인터페이스 (이것만 선언하면 모든 도메인에 대응 가능)
public interface ISignalHub<TMarker> where TMarker : ISignalMarker
{
    public void Publish<T>(T signal) where T : struct;
    public void Publish<TContext, TData>(TContext context, ReadOnlySpan<TData> data) where TContext : struct;


    void Subscribe<T>(Action<T> handler) where T : struct, TMarker;
    void UnSubscribe<T>(Action<T> handler) where T : struct, TMarker;

    void Subscribe<TContext, TData>(SignalHub.SpanHandler<TContext, TData> handler)
        where TContext : struct, TMarker;
    void UnSubscribe<TContext, TData>(SpanHandler<TContext, TData> handler)
        where TContext : struct, TMarker;

    void BeginScope<T>(T context) where T : struct, ISignalMarker;
    void EndScope<T>(T context) where T : struct, ISignalMarker;

    void SubscribeScope<T>(Action<ScopeSignal<T>> handler) where T : struct, TMarker;
    void UnSubscribeScope<T>(Action<ScopeSignal<T>> handler) where T : struct, TMarker;
}

public class SignalHub : ISignalHub<ICardSystemPrivateSignal>, ISignalHub<IPulicSignal>
{
    // --- [내부 구조 정의] ---
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

    // ---------------------------------------------------------
    // [Private 로직] 실제 처리는 여기서 딱 한 번만 정의됩니다.
    // ---------------------------------------------------------

    private void DoSubscribe<T>(Action<T> handler) where T : struct
    {
        var list = GetOrCreateList<T>();
        if (!list.Handlers.Contains(handler)) list.Handlers.Add(handler);
    }

    private void DoUnSubscribe<T>(Action<T> handler) where T : struct
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            ((SignalHandlerList<T>)listObj).Handlers.Remove(handler);
        }
    }

    private void DoSubscribeSpan<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct
    {
        var list = GetOrCreateSpanList<TContext, TData>();
        if (!list.Handlers.Contains(handler)) list.Handlers.Add(handler);
    }

    private void DoUnSubscribeSpan<TContext, TData>(SpanHandler<TContext, TData> handler) where TContext : struct
    {
        var key = (typeof(TContext), typeof(TData));
        if (_spanStorage.TryGetValue(key, out var listObj))
        {
            ((SpanSignalHandlerList<TContext, TData>)listObj).Handlers.Remove(handler);
        }
    }

    // ---------------------------------------------------------
    // [인터페이스 구현] 한 줄씩 포워딩하여 중복 제거
    // ---------------------------------------------------------

    // --- CardSystem (ICardSignal) ---
    void ISignalHub<ICardSystemPrivateSignal>.Subscribe<T>(Action<T> h) => DoSubscribe(h);
    void ISignalHub<ICardSystemPrivateSignal>.UnSubscribe<T>(Action<T> h) => DoUnSubscribe(h);
    void ISignalHub<ICardSystemPrivateSignal>.Subscribe<TC, TD>(SpanHandler<TC, TD> h) => DoSubscribeSpan<TC, TD>(h);
    void ISignalHub<ICardSystemPrivateSignal>.UnSubscribe<TC, TD>(SpanHandler<TC, TD> h) => DoUnSubscribeSpan<TC, TD>(h);

    // --- CombatSystem (ICombatSignal) ---
    void ISignalHub<IPulicSignal>.Subscribe<T>(Action<T> h) => DoSubscribe(h);
    void ISignalHub<IPulicSignal>.UnSubscribe<T>(Action<T> h) => DoUnSubscribe(h);
    void ISignalHub<IPulicSignal>.Subscribe<TC, TD>(SpanHandler<TC, TD> h) => DoSubscribeSpan<TC, TD>(h);
    void ISignalHub<IPulicSignal>.UnSubscribe<TC, TD>(SpanHandler<TC, TD> h) => DoUnSubscribeSpan<TC, TD>(h);

    // 카드 시스템 스코프 구독
    void ISignalHub<ICardSystemPrivateSignal>.SubscribeScope<T>(Action<ScopeSignal<T>> h) => DoSubscribe(h);
    void ISignalHub<ICardSystemPrivateSignal>.UnSubscribeScope<T>(Action<ScopeSignal<T>> h) => DoUnSubscribe(h);

    // 범용 스코프 구독
    void ISignalHub<IPulicSignal>.SubscribeScope<T>(Action<ScopeSignal<T>> h) => DoSubscribe(h);
    void ISignalHub<IPulicSignal>.UnSubscribeScope<T>(Action<ScopeSignal<T>> h) => DoUnSubscribe(h);

    // ---------------------------------------------------------
    // [Public] Publish는 발행자의 권한이므로 제약 없이 오픈
    // ---------------------------------------------------------

    public void Publish<T>(T signal) where T : struct
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            var handlers = ((SignalHandlerList<T>)listObj).Handlers;
            for (int i = handlers.Count - 1; i >= 0; i--) handlers[i]?.Invoke(signal);
        }
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

    // --- 스코프 메서드 ---
    public void BeginScope<T>(T context) where T : struct, ISignalMarker
    {
        // ScopeSignal<T>은 내부 신호이므로 직접 Publish 로직 호출
        PublishInternal(new ScopeSignal<T> { IsBegin = true, Context = context });
    }

    public void EndScope<T>(T context) where T : struct, ISignalMarker
    {
        PublishInternal(new ScopeSignal<T> { IsBegin = false, Context = context });
    }

    // 내부 전용 발행 로직 (마커 제약 없음)
    private void PublishInternal<T>(T signal) where T : struct
    {
        if (_storage.TryGetValue(typeof(T), out var listObj))
        {
            var handlers = ((SignalHandlerList<T>)listObj).Handlers;
            for (int i = handlers.Count - 1; i >= 0; i--) handlers[i]?.Invoke(signal);
        }
    }

    // --- 헬퍼 메서드 ---
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

    private SpanSignalHandlerList<TContext, TData> GetOrCreateSpanList<TContext, TData>()
    {
        var key = (typeof(TContext), typeof(TData));
        if (!_spanStorage.TryGetValue(key, out var list))
        {
            list = new SpanSignalHandlerList<TContext, TData>();
            _spanStorage[key] = list;
        }
        return (SpanSignalHandlerList<TContext, TData>)list;
    }
}