namespace Home.Common.Messages;

public sealed class StockChangedMessage : BaseMessage
{
    public const string TopicName = "inventory.stock.changed";

    public StockChangedMessage() : base(TopicName)
    {
    }

    public string StockId { get; set; }
    public string ProductId { get; set; }
    public decimal QuantityNew { get; set; }
    public decimal QuantityOpened { get; set; }
}
