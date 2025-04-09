using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Server.Authorization;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Auth;
using ServerLibrary.Services.Implementations;
using ServerLibrary.Services.Implementations.Auth;
using System.Text;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // сначала уровень
                .WriteTo.Console()
                .WriteTo.File("logs/info_log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File("logs/warning_log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.File("logs/error_log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();

            builder.Host.UseSerilog();

            #endregion

            #region DB Connect
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .LogTo(Console.Write, LogLevel.Information)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            #endregion

            #region Registration Services
            builder.Services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            #region SwaggerGen
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Online Shop API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Просто вставьте токен сюда)"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

            });
            #endregion

            builder.Services.AddMyAuth();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "OnlineStore",
                        ValidAudience = "OnlineStoreUsers",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("idfjrhcgquegh1c9p4rhqw08ex,fhamisudhfvauipsrghcairhfxajshdfjxhasdfh"))
                    };
                });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            ;
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthentication(); // обязательно ДО UseAuthorization
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
