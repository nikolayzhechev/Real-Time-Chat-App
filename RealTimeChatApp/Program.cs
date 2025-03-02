using RealTimeChatApp.Hubs;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // add SignalR service
        builder.Services.AddSignalR();

        var app = builder.Build();

        // HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapHub<ChatHub>("/hub");

        app.Run();
    }
}