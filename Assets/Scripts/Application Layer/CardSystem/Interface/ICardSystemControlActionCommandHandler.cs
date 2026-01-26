using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler
{
    void UseCardsAndExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards);
    void InsertFollowUpEffectCommand(CardEffectCommand command);
}
