using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Casino_Royale_Api.Data;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Services;

namespace Casino_Royale_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString;

            if (HostingEnvironment.IsDevelopment() || 
                HostingEnvironment.IsEnvironment("Testing"))
            {
                connectionString = Configuration.GetConnectionString("localDb");
            }
            else
            {
                connectionString = string.Format(Configuration.GetConnectionString("awsDb"));
            }

            if (HostingEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<CasinoDbContext>(options => options.UseSqlite(connectionString));
            }
            else
            {
                services.AddDbContext<CasinoDbContext>(options => options.UseSqlServer(connectionString));
            }
            
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IGameManager, GameManager>();
            services.AddAutoMapper(typeof(PlayerProfile));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CasinoDbContext context)
        {
            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            context.Database.EnsureCreated();
        }
    }
}
