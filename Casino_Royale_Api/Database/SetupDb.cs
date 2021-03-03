using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Casino_Royale_Api.Data
{
  public static class SetupDb
  {
    public static void SetupConfig(IApplicationBuilder app, IWebHostEnvironment env)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        seedDb(serviceScope.ServiceProvider.GetService<CasinoDbContext>(), env);
      }
    }
    public static void seedDb(CasinoDbContext context, IWebHostEnvironment env)
    {
      System.Console.WriteLine("Applying Migrations...");
      context.Database.Migrate();
      context.SaveChanges();
    }
  }
}
