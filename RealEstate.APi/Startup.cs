using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RealEstate.APi.Infrastructer;
using RealEstate.Service;
using RealEstate.Service.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.APi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // dependency injection was applied to the Auto Mapper.
            var _mappingProfile = new MapperConfiguration(mp => { mp.AddProfile(new MappingProfile()); });
            IMapper mapper = _mappingProfile.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<LoginFilter>();

            services.AddStackExchangeRedisCache(options => options.Configuration = "localhost:6379");
            //dependency injection was applied to the services.
            services.AddSingleton<IRealEstateOwnerService, RealEstateOwnerService>();
            services.AddSingleton<IRealEstateService, RealEstateService>();

            services.AddHangfire(configuration =>
                     configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                     .UseSimpleAssemblyNameTypeSerializer()
                     .UseDefaultTypeSerializer()
                     .UseMemoryStorage());

            services.AddMemoryCache();
            services.AddSingleton<IPrintWelcomeJob, PrintWelcomeJob>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstate.APi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              IBackgroundJobClient backgroundJobClient,
                              IRecurringJobManager recurringJobManager,
                              IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstate.APi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard();
            backgroundJobClient.Enqueue(() => Console.WriteLine("Hello Hangfire Job"));
            recurringJobManager.AddOrUpdate(
                "Run every minute",
                () => new PrintWelcomeJob().PrintWelcome(),
                "* * * * *"
            );

            recurringJobManager.AddOrUpdate(
                 "Run every minute",
                 () => serviceProvider.GetService<IPrintWelcomeJob>().CleanUserTable(),
                 "* * * * *"
                 );
        }
    }
}
