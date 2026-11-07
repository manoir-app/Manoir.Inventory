using MaNoir.Core.Contracts.Models.Contributions;

namespace MaNoir.Inventory;

public static class InventoryPluginDescriptorProvider
{
    public const string PluginId = "inventory";
    public const string RepositoryUrl = "https://github.com/manoir-app/Manoir.Inventory";

    public static PluginDescriptor Create(string version)
    {
        string resolvedVersion = string.IsNullOrWhiteSpace(version) ? "0.0.0" : version;

        return new PluginDescriptor()
        {
            Id = PluginId,
            Label = "Inventory",
            Version = resolvedVersion,
            Description = "Household stock and possessions: consumables, assets, and their storage.",
            Publisher = "MaNoir",
            RepositoryUrl = RepositoryUrl,
            DependencyRepositoryUrls = [],
            AccessZones = [],
            Contributions = [],
        };
    }
}
