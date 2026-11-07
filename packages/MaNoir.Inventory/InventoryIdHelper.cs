using System;

namespace MaNoir.Inventory;

// Every inventory entity uses a Guid rendered as a lowercase string, matching the legacy home-automation convention.
public static class InventoryIdHelper
{
    public static string NewId()
    {
        return Guid.NewGuid().ToString("D").ToLowerInvariant();
    }

    public static string NormalizeId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return id.Trim().ToLowerInvariant();
    }
}
