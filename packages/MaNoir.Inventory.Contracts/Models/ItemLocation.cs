namespace MaNoir.Inventory.Contracts.Models;

public enum PrivacyLevel
{
    Public,
    Household,
    Private
}

// Zone > Room > Container > free text, shared by ProductStock and Asset.
public class ItemLocation
{
    public string ZoneId { get; set; }
    public string RoomId { get; set; }
    public string ContainerId { get; set; }
    public string ContainerSubElementId { get; set; }
    public string FreeText { get; set; }
}
