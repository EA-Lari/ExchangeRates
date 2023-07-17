using Automatonymous;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Events;
using ExchangeTypes.Saga;
using MassTransit;
using MassTransit.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Core;
using Storage.Core.Repositories;
using Storage.Core.Services;
using Storage.Database;
using System;

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
                .AddTransient<CurrencyService>();
            //.AddTransient<ICurrencyHandler<UpdateCurrencyInfoEvent>, UpdateCurrencyInfoHandler>()
            //.AddTransient<ICurrencyHandler<UpdateCurrencyRateEvent>, UpdateCurrencyRateHandler>();
            //.AddSingleton<MassTransitStateMachine <CurrencyState>, CurrencyStateMachine >();

            // var repository = new InMemorySagaRepository<OrderState>();




            Console.WriteLine("Test");




            services.AddMassTransit(x =>
            {
                //x.AddConsumersFromNamespaceContaining();
                x.AddSagaStateMachine<CurrencyStateMachine, CurrencyState>()
                    .InMemoryRepository();
                x.UsingRabbitMq((context, cfg) =>
                {
                    //cfg.Message<UpdateCurrencyInfoEvent>(x => x.SetEntityName("UpdateCurrencyInfo"));
                    //cfg.Message<UpdateCurrencyRateEvent>(x => x.SetEntityName("UpdateCurrencyRate"));
                    cfg.Host(_configuration.GetSection("Rabbit").Value);
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
            Console.WriteLine("AFTER connect Migra tion");
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
