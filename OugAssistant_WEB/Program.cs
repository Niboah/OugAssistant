using Microsoft.EntityFrameworkCore;

namespace OugAssistant_WEB;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(); // Add API controllers
        builder.Services.AddEndpointsApiExplorer();  // API documentation (Swagger)
        builder.Services.AddSwaggerGen(); // Enable Swagger UI for API documentation

        builder.Services.AddMvc();           // Add MVC services (MVC controllers, views, etc.)

        builder.Services.AddDbContext<OugAssistant_DB.Features.Planning>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("PlanningConnection") ?? throw new InvalidOperationException("Connection string 'PlanningConnection' not found.")));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();  // Show detailed exception page in development
            app.UseSwagger();                 // Enable Swagger middleware
            app.UseSwaggerUI();               // Display Swagger UI

        }
        else
        {
            app.UseExceptionHandler("/Home/Error");  // Handle errors in production
            app.UseHsts();  // Enforce HTTP Strict Transport Security
        }

        app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
        app.UseStaticFiles();       // Serve static files like images, JS, and CSS

        app.UseRouting();  // Enable routing for MVC and API controllers

        app.UseAuthorization();

        app.MapControllers();  // Map API routes to controllers
        app.MapControllerRoute(  // Map MVC routes (views)
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
