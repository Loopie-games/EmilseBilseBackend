using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.DataAccess.Repositories;
using moonbaboon.bingo.Domain.IRepositories;
using moonbaboon.bingo.Domain.Services;
using moonbaboon.bingo.WebApi.SignalR;
using MySqlConnector;

namespace moonbaboon.bingo.WebApi
{
    public class Startup
    {
        private static string POLICY_DEV = "dev-cors";
        private static string POLICY_PROD = "prod-cors";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo {Title = "EmilseBilseBingo.WebApi", Version = "v1"});
            });

            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy(POLICY_DEV, policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .WithOrigins("http://185.51.76.204:9070/")
                        .AllowCredentials();
                });
                options.AddPolicy(POLICY_PROD, policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://185.51.76.204:9071/")
                        .AllowCredentials();
                });
            });

            //Setting up dependency injection

            //Users
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            
            //Tiles
            services.AddScoped<ITileRepository, TileRepository>();
            services.AddScoped<ITileService, TileService>();
            
            //Friendships
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendshipService, FriendshipService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseCors(env.IsProduction() ? POLICY_PROD : POLICY_DEV);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }
}