using RealTimeChatApp.Hubs;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add controllers to the service container
        builder.Services.AddControllers();

        /*
        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
        });
        */
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
        app.UseAuthorization();
        app.MapControllers();

        app.MapHub<ChatHub>("/hub");

        app.Run();
    }
}