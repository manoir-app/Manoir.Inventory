using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MaNoir.Core.Contributions;

namespace MaNoir.Inventory.Api;

public static class InventoryApiModule
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(InventoryApiModule).Assembly);
    }

    public static void ConfigureApplication(WebApplication app)
    {
        string version = null;
        if (typeof(InventoryApiModule).Assembly.GetName().Version != null)
            version = typeof(InventoryApiModule).Assembly.GetName().Version.ToString(3);

        new PluginRegistrationLogic()
            .PublishPluginDescriptorAsync(InventoryPluginDescriptorProvider.Create(version))
            .GetAwaiter()
            .GetResult();

        app.MapControllers();
    }
}
