using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interaces.Services.AppSettings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Plant.Api.Services;

namespace Plant.Api {
    public class Startup {

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup (IConfiguration configuration, ILogger<Startup> logger) {
            Configuration = configuration;
            Logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {

            services.AddCors (options => {
                options.AddPolicy (MyAllowSpecificOrigins,
                    builder => {
                        builder.WithOrigins ().AllowAnyOrigin ();
                    });
            });

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new OpenApiInfo { Title = "Plant api", Version = "v1" });
            });

            services.AddSingleton<IConfiguration> (Configuration);
            services.AddSingleton<ILogger> (Logger);

            services.AddTransient<IAppSettings, AppSettings> ();

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
        }

        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger ();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI (c => {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Plant api V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors (MyAllowSpecificOrigins);

            app.UseMvc ();
        }
    }
}