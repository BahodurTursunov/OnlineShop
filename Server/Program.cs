#region Usages
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using ServerLibrary.Data;
using ServerLibrary.DI;
using ServerLibrary.Helpers;
using ServerLibrary.Middleware;
using ServerLibrary.SignalR;
using System.Security.Claims;
using System.Text;
#endregion


namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region JWT
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            var jwtSettings = jwtSection.Get<JwtSettings>();
            #endregion

            #region Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // ������� �������
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
            builder.Services.AddMyServices();
            #endregion

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                });
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
                    Description = "������ �������� ����� ����)"
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
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };
                });


            var app = builder.Build();

            app.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapHub<SupportHub>("/support", options =>
            {
                options.ApplicationMaxBufferSize = 128;
                options.TransportMaxBufferSize = 128;
                options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            ;
            app.UseRouting();
            // app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ApiResponseMiddleware>();

            app.MapControllers();
            app.Run();
        }
    }
}
