using System;

namespace Home.Common.Messages;

public sealed class StockExpiringMessage : BaseMessage
{
    public const string TopicName = "inventory.stock.expiring";

    public StockExpiringMessage() : base(TopicName)
    {
    }

    public string StockId { get; set; }
    public string ProductId { get; set; }
    public DateTimeOffset DateExpiration { get; set; }
}
