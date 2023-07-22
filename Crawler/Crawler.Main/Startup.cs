using Automatonymous;
using Crawler.Core;
using Crawler.Core.Interfaces;
using Crawler.Core.Models;
using Crawler.Database;
using Crawler.Main.Handlers;
using Crawler.Main.Models;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using ExchangeTypes.Saga;
using GreenPipes;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
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
            // Add Hangfire services.
            //services.AddHangfire((provider, configuration) => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UsePostgreSqlStorage(_configuration.GetConnectionString("HangfireConnection")));

            var connectionString = _configuration.GetConnectionString("OperationPointConnection");
            //services.AddDbContext<OperationPointContext>(options =>
            //    options.UseNpgsql(connectionString));
            // Add the processing server as IHostedService
           // services.AddHangfireServer();
            services.AddAutoMapper(x => x.AddProfile(new CurrancyProfile()));
            services.AddTransient<CurrencyService>()
                .AddTransient<CurrencyPublisher>()
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
           // context.Database.Migrate();
            //using (var conn = (NpgsqlConnection)context.Database.GetDbConnection())
            //{
            //    conn.Open();
            //    conn.ReloadTypes();
            //}
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //BackgroundJob.Enqueue<CurrencyService>(x => x.LoadCurrencyInfos());
            //var interval = _configuration.GetSection(nameof(Interval))
            //    .Get<Interval>();
            //RecurringJob.AddOrUpdate<CurrencyService>("UpdateCurrencies", ms => ms.LoadCurrencyTodayRates(), interval.Cron);
            
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
