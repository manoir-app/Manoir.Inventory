using System.Collections.Generic;

namespace MaNoir.Inventory.Contracts.Models;

public enum ContainerRole
{
    None,
    Fridge,
    Freezer
}

// A physical storage location (cupboard, bin, shelf) inside a room, shared by stock and possessions.
public class Container
{
    public Container()
    {
        AllowedProductMetaTypes = new List<ProductMetaType>();
        AssociatedDeviceIds = new Dictionary<string, string>();
        SubElements = new List<ContainerSubElement>();
        Audit = new AuditInfo();
    }

    public string Id { get; set; }
    public string ZoneId { get; set; }
    public string RoomId { get; set; }
    public string Label { get; set; }
    public ContainerRole Role { get; set; }
    public string Notes { get; set; }
    public decimal CoordinateX { get; set; }
    public decimal CoordinateY { get; set; }

    public ContainerDisplayShape ShapeOnFloorPlan { get; set; }
    public ContainerDisplayShape ShapeDetails { get; set; }

    // Only meaningful when the container stores consumables; left empty for asset-only containers.
    public List<ProductMetaType> AllowedProductMetaTypes { get; set; }
    public List<ContainerSubElement> SubElements { get; set; }

    public Dictionary<string, string> AssociatedDeviceIds { get; set; }

    public AuditInfo Audit { get; set; }
}

public class ContainerDisplayShape
{
    public ContainerDisplayShape()
    {
        Shape = new List<LocationPoint>();
        Images = new Dictionary<string, string>();
    }

    public List<LocationPoint> Shape { get; set; }
    public Dictionary<string, string> Images { get; set; }
}

public class LocationPoint
{
    public decimal X { get; set; }
    public decimal Y { get; set; }
}

public class ContainerSubElement
{
    public ContainerSubElement()
    {
        AllowedProductMetaTypes = new List<ProductMetaType>();
    }

    public string Id { get; set; }
    public string Label { get; set; }
    public ContainerRole Role { get; set; }
    public ContainerDisplayShape ShapeInContainer { get; set; }
    public decimal CoordinateX { get; set; }
    public decimal CoordinateY { get; set; }
    public List<ProductMetaType> AllowedProductMetaTypes { get; set; }
}
