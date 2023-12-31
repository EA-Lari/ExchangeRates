using ExchangeTypes.Request;
using ExchangeTypes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Core;
using Storage.Core.Handlers;
using Storage.Core.Repositories;
using Storage.Database;
using System;
using ExchangeTypes.Consumers;
using GreenPipes;
using ExchangeTypes.DTO;

namespace Storage.Main
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("CurrencyContext");

            services.AddAutoMapper(x => x.AddProfile(new CurrancyProfile()));

            services.AddDbContext<CurrencyContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddTransient<CurrencyRatesRepository>()
                .AddTransient<ICurrencyHandler<UpdateCurrencyRequest, UpdateCurrencyResponce>, UpdateCurrencyInfoHandler>()
                .AddTransient<ICurrencyHandler<UpdateRatesRequest, UpdateRatesResponce>, UpdateCurrencyRateHandler>()
                .AddTransient<ICurrencyHandler<FilterByCurrencyDto, CurrencyRateResponce>, GetCurrencyHandler>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddDelayedMessageScheduler();
                x.AddConsumer<UpdateCurrencyConsumer>();
                x.AddConsumer<UpdateRatesConsumer>();
                x.AddConsumer<GetCurrencyConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseInMemoryOutbox();

                    cfg.UseDelayedMessageScheduler();
                    cfg.Host(_configuration.GetSection("Rabbit").Value);

                    cfg.UseMessageRetry(r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransitHostedService();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CurrencyContext context)
        {
            Console.WriteLine("BEFOR connect Migration");
            context.Database.Migrate();
            Console.WriteLine("AFTER connect Migration");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
