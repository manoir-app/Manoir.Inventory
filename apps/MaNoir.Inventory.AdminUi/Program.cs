using Microsoft.AspNetCore.Builder;
using MaNoir.Core.AdminUi.Hosting;
using MaNoir.Inventory.Api;

namespace MaNoir.Inventory.AdminUi;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.AddMaNoirAdminUiHosting();
        InventoryApiModule.ConfigureBuilder(builder);

        WebApplication app = builder.Build();
        app.UseMaNoirAdminUiHosting();
        InventoryApiModule.ConfigureApplication(app);

        app.Run();
    }
}
