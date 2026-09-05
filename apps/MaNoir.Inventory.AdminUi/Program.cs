using Microsoft.AspNetCore.Builder;
using MaNoir.Inventory.Api;

namespace MaNoir.Inventory.AdminUi;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        InventoryApiModule.ConfigureBuilder(builder);

        WebApplication app = builder.Build();
        InventoryApiModule.ConfigureApplication(app);

        app.Run();
    }
}
