using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OuterWorlds.Api.Infrastructure.IoC;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Api
{
    public class Startup
    {
        public Container _dependencyInjectionContainer { get; } = new Container();
        public Bootstrapper _bootstrapper { get; } = new Bootstrapper();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAllHeaders",
                    builder =>
                    {
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Outer Worlds", Version = "v1" });
            });

            _dependencyInjectionContainer.Options.AllowOverridingRegistrations = true;
            _dependencyInjectionContainer.Options.DefaultLifestyle = new AsyncScopedLifestyle();

            services.AddSimpleInjector(_dependencyInjectionContainer, opt =>
            {
                opt.AddAspNetCore()
                    .AddControllerActivation();

                opt.AddLogging();
            });

            services.AddSingleton(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("AllowAllHeaders");
            _bootstrapper.Inject(_dependencyInjectionContainer);
            app.UseSimpleInjector(_dependencyInjectionContainer);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Outer Worlds");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller}/{action}");
            });

            _dependencyInjectionContainer.Verify();
        }
    }
}
