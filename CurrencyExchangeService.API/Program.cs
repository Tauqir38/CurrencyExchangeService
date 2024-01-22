using CurrencyExchangeService.Business.Models;
using CurrencyExchangeService.Business.Services;
using CurrencyExchangeService.Data.DBContext;
using CurrencyExchangeService.Data.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CurrencyExchangeService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .WriteTo.Console()
                 .WriteTo.File("logs/currencyexchangeservice.txt", rollingInterval: RollingInterval.Day)
                 .CreateLogger();

            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();
            builder.Services.Configure<ExchangeRateServiceConfig>(builder.Configuration.GetSection("ExchangeRateServiceConfig"));
            builder.Services.AddScoped<ICurrencyExchangeTradeRepository, CurrencyExchangeTradeRepository>();
            builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
            builder.Services.AddDbContext<ExchangeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ExchangeDatabase")));
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature != null)
                    {
                        var exception = exceptionHandlerFeature.Error;
                        Log.Error(exception, "Unhandled exception");

                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("An error occurred processing your request.");
                    }
                });
            });

            app.UseHttpsRedirection();

            app.MapControllers();

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}