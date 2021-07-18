using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Modules;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace NaturalPersonsDirectory.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddMvc().AddFluentValidation(
                config => config.RegisterValidatorsFromAssembly(Assembly.Load("NaturalPersonsDirectory.Modules")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "NPDirectory.Api", Version = "v1"});
            });
            
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddTransient<INaturalPersonService, NaturalPersonService>();
            services.AddTransient<IRelationService, RelationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NPDirectory.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            Seeder.Seed(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
