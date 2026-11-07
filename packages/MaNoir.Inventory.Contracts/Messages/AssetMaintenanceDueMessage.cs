using System;

namespace Home.Common.Messages;

public sealed class AssetMaintenanceDueMessage : BaseMessage
{
    public const string TopicName = "inventory.asset.maintenancedue";

    public AssetMaintenanceDueMessage() : base(TopicName)
    {
    }

    public string AssetId { get; set; }
    public string ScheduleId { get; set; }
    public string Kind { get; set; }
    public DateTimeOffset? NextDueOn { get; set; }
}
