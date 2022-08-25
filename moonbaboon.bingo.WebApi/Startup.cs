using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/game")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                            
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            
            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "EmilseBilseBingo.WebApi", Version = "v1"});
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
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
                        .WithOrigins("http://localhost:9070")
                        .WithOrigins("http://localhost:9090")
                        .WithOrigins("http://185.51.76.204:9070")
                        .WithOrigins("http://185.51.76.204:9090")
                        .AllowCredentials();
                });
                options.AddPolicy(POLICY_PROD, policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .WithOrigins("http://localhost:9071")
                        .WithOrigins("http://185.51.76.204:9071")
                        .AllowCredentials();
                });
            });

            //Setting up dependency injection
            //Users
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            
            //Auth
            services.AddScoped<IAuthService, AuthService>();
            
            //Tiles
            services.AddScoped<IUserTileRepository, UserTileRepository>();
            services.AddScoped<IUserTileService, UserTileService>();
            
            //Friendships
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            
            //Lobby
            services.AddScoped<ILobbyRepository, LobbyRepository>();
            services.AddScoped<ILobbyService, LobbyService>();
            
            //PendingPlayer
            services.AddScoped<IPendingPlayerRepository, PendingPlayerRepository>();
            services.AddScoped<IPendingPlayerService, PendingPlayerService>();
            
            //Game
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameService, GameService>();
            
            //Board
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<IBoardService, BoardService>();
            
            //BoardTile
            services.AddScoped<IBoardTileRepository, BoardTileRepository>();
            services.AddScoped<IBoardTileService, BoardTileService>();
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
            if (env.IsProduction())
            {
                app.UseCors(POLICY_PROD);
            }
            else
            {
                app.UseCors(POLICY_DEV);
            }
            

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }
}
