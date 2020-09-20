using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Modules;
using System.Reflection;

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
            services.AddMvc().AddFluentValidation(config => config.RegisterValidatorsFromAssembly(Assembly.Load("NaturalPersonsDirectory.Modules")));
            services.AddDbContext<NaturalPersonsDirectoryDbContext>(options => options.UseSqlServer(GlobalVariables.DbConnectionString));
            services.AddTransient<INaturalPersonService, NaturalPersonService>();
            services.AddTransient<IRelationService, RelationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            Seeder.Seed(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
