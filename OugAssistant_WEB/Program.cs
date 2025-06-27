using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using static OugAssistant_DB.Features.PlanningDBContext;

namespace OugAssistant_WEB;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");

        try{
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers() // Add API controllers
                        .AddJsonOptions(options =>
                        {
                            options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                        });

            builder.Services.AddEndpointsApiExplorer();  // API documentation (Swagger)
            builder.Services.AddSwaggerGen(); // Enable Swagger UI for API documentation

            builder.Services.AddMvc();           // Add MVC services (MVC controllers, views, etc.)

            builder.Services.AddDbContext<OugAssistant_DB.Features.PlanningDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("PlanningConnection") ?? throw new InvalidOperationException("Connection string 'PlanningConnection' not found.")));

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();  // Show detailed exception page in development
                
                app.UseSwaggerUI();               // Display Swagger UI
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");  // Handle errors in production
                app.UseHsts();  // Enforce HTTP Strict Transport Security
            }

            app.UseSwagger();                 // Enable Swagger middleware

            app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
            app.UseStaticFiles();       // Serve static files like images, JS, and CSS

            app.UseRouting();  // Enable routing for MVC and API controllers

            app.UseAuthorization();

            app.MapControllers();  // Map API routes to controllers
            app.MapControllerRoute(  // Map MVC routes (views)
                name: "default",
                pattern: "{controller=Planning}/{action=Index}/{id?}");

            app.Run();

        }
        catch (Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
       
    }
}
