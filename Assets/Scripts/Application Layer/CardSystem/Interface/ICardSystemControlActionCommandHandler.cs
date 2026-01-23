using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler
{
    void UseCardnExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards);
}
