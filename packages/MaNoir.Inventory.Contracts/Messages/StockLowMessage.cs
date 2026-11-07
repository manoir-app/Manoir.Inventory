namespace Home.Common.Messages;

public sealed class StockLowMessage : BaseMessage
{
    public const string TopicName = "inventory.stock.low";

    public StockLowMessage() : base(TopicName)
    {
    }

    public string StockId { get; set; }
    public string ProductId { get; set; }
    public decimal QuantityNew { get; set; }
    public decimal QuantityOpened { get; set; }
}
