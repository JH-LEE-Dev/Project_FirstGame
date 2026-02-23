using System;
using System.Collections.Generic;

public interface IComplexSystemActionCommandHandler : ICommandHandler
{
    ICardLogicSystemActionCommandHandler cardLogicSystem { get; }
    IStatusEffectCommandHandler statusSystem { get; }
    ICardDataControlActionCommandHandler cardDataSystem { get; }
    ICardFlowDataActionCommandHandler cardFlowSystem { get; }
    ICardSlotSystemActionCommandHandler cardSlotSystem { get; }
    ICardSelectionSystemActionCommandHandler cardSelectionSystem { get; }
    ICardSystemControlActionCommandHandler cardSystem { get; }
}
