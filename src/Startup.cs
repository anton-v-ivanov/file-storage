using System;
using System.Linq;
using FileStorage.Authentication;
using FileStorage.Dispatchers;
using FileStorage.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StatsdClient;

namespace FileStorage
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddTransient<IFilesDispatcher, FilesDispatcher>();
            services.AddTransient<IFilesRepository, FilesRepository>();
            services.AddTransient<IAuthValidator, AuthValidator>();
            services.AddLogging();
            services.AddMvc();

            services.Configure<AuthenticationOptions>(opt =>
                {
                    opt.Resources = Configuration.GetSection("AuthenticationOptions:Resources")
                        .GetChildren()
                        .ToDictionary(s => s.Key, section => section.Value);
                    opt.Parse();
                });

            services.Configure<UploadConfiguration>(Configuration.GetSection("Upload"));

            Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = Configuration.GetSection("Metrics:StatsdServerName").Value,
                Prefix = Configuration.GetSection("Metrics:Prefix").Value
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddFile(Configuration["Logging:File"]);
            loggerFactory.AddDebug();

            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseMvc();
        }
    }
}
