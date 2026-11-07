using MaNoir.Core.Contracts.Models.Contributions;
using MaNoir.Core.Contracts.Models.Authorization;
using System.Collections.Generic;

namespace MaNoir.Inventory;

public static class InventoryPluginDescriptorProvider
{
    public const string PluginId = "inventory";
    public const string RepositoryUrl = "https://github.com/manoir-app/Manoir.Inventory";
    public const string AdminUiAccessZoneId = "inventory.admin-ui";

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
            AccessZones =
            [
                new AccessZoneDefinition()
                {
                    Id = AdminUiAccessZoneId,
                    Label = "Inventory administration",
                    Description = "Access to the Inventory administration pages."
                }
            ],
            // Keep in sync with catalog.contributions in manoir.plugin.yaml.
            Contributions =
            [
                new ContributionDefinition()
                {
                    Id = "inventory.admin.pages",
                    Kind = ContributionKind.AdminUiPage,
                    Label = "Inventory",
                    Description = "Inventory administration pages.",
                    CanCreateInstances = false,
                    CanInstallMultipleTimes = false,
                    Tags = ["inventory"],
                    AdminUi = new AdminUiContributionDefinitionData()
                    {
                        Domain = "Inventory",
                        AccessZoneId = AdminUiAccessZoneId,
                        RequiredAccessLevel = AccessLevel.Read,
                        Pages =
                        [
                            Page("Inventory", "Overview", "/inventory/overview", "Overview", "Vue d'ensemble"),
                            Page("Stock", "Products", "/inventory/stock/products", "Products", "Produits"),
                            Page("Stock", "LowStock", "/inventory/stock/low", "Low stock and expiring", "Stock bas et peremptions"),
                            Page("Possessions", "Assets", "/inventory/possessions/assets", "Assets and equipment", "Biens et equipements"),
                            Page("Organisation", "Containers", "/inventory/organisation/containers", "Storage locations", "Rangements"),
                            Page("Organisation", "Categories", "/inventory/organisation/categories", "Categories", "Categories"),
                        ]
                    }
                }
            ],
        };
    }

    private static AdminUiPageDefinition Page(string category, string name, string relativePath, string labelEn, string labelFr)
    {
        return new AdminUiPageDefinition()
        {
            Category = category,
            Name = name,
            RelativePath = relativePath,
            Labels = new Dictionary<string, string>()
            {
                ["en"] = labelEn,
                ["fr-FR"] = labelFr
            }
        };
    }
}
