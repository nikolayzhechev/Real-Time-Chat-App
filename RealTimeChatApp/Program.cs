using RealTimeChatApp.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Services;
using RealTimeChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NSwag.Generation.Processors.Security;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add controllers to the service container
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(config =>
        {
            config.Title = "RTCA";
            config.Version = "v1";
            config.Description = "Real-Time Chat Application API Documentation";
            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT", new NSwag.OpenApiSecurityScheme
            {
                Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            }));
        });

        // EF db context registration
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        var key = builder.Configuration["Jwt:Key"];
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        // Add auth service
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key))
                };
                // Allows SignalR to get the token from the query string
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddScoped<IEmailService, EmailService>();

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IChatService, ChatService>();

        // Allow CORS for React
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReact",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:7250") // React Dev Server
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
        });

        // Add SignalR service
        builder.Services.AddSignalR();

        var app = builder.Build();

        app.UseCors("AllowReact");  // Ensure CORS middleware is used

        // HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Redirect to index.html if no route is specified
        app.UseDefaultFiles();
        // Serve static files from wwwroot
        app.UseStaticFiles();

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
            RequestPath = "/uploads"
        });
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ChatHub>("/hub").RequireAuthorization();

        // Apply EF db migrations at runtime
        using (var scope = app.Services.CreateScope())
        {
            if (app.Environment.IsDevelopment())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();

                app.UseOpenApi();
                app.UseSwaggerUi();
            }
        }


        app.MapControllers();
        app.Run();
    }
}