using Crawler.Core;
using Crawler.Core.Interfaces;
using Crawler.Core.Models;
using Crawler.Main.Handlers;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Request;
using ExchangeTypes.Saga;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace Crawler.Main
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
            var connectionString = _configuration.GetConnectionString("OperationPointConnection");

            services.AddAutoMapper(x => x.AddProfile(new CurrancyProfile()));
            services.AddTransient<ICurrencyService, CurrencyService>()
                    .AddTransient<ICurrencyHandler<GetActualCurrencyRequest, GetActualCurrencyResponce>, GetActualCurrencyHandler>();

            services.AddHttpClient<ICrawlerClientService, CrawlerClientService>();

            services.AddControllers(options =>
                    options.Filters.Add(typeof(ExceptionFilter)));

            services.Configure<UrlCurrency>(_configuration.GetSection(nameof(UrlCurrency)));

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddDelayedMessageScheduler();
                x.AddSagaStateMachine<CurrencyStateMachine, CurrencyState>()
                    .InMemoryRepository();

                x.AddConsumer<ActualCurrencyConsumer>();
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

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
