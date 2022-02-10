using API.Middlewares;
using API.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PrintServerMgmtWebApi.Repository;
using System;

namespace PrintServerMgmtWebApi
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
            //services.AddControllersWithViews(options =>
            //{
            //    options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            //});
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddScoped<PrintServerManagementScope>();
            services.AddScoped<IPrintersRepository, PrintersRepositoryWMI>();
            services.AddScoped<PrintersService>();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(string.Format(@"{0}\PrintServerMgmtWebApi.xml", AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PrintServerMgmtWebApi",
                });
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            //services.AddSingleton<IClaimsTransformation, ADClaimsTransformer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDeveloperExceptionPage();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<SwaggerAuthorizationMiddleware>();
            

            //app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrintServerMgmtWebApi");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
