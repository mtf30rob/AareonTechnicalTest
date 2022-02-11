using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AareonTechnicalTest
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
            services.AddControllers();

            var envDir = Environment.CurrentDirectory;
            var databasePath = $"{envDir}{System.IO.Path.DirectorySeparatorChar}Ticketing.db";
            services.AddDbContext<ApplicationContext>(c => c.UseSqlite($"Data Source={databasePath}"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AareonTechnicalTest", Version = "v1", Description = GetDescriptionMarkdown() });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AareonTechnicalTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static string GetDescriptionMarkdown()
        {
            var descriptionMarkdown = new StringBuilder();
            descriptionMarkdown.AppendLine("## Welcome to the Aareon Technical Test API");
            descriptionMarkdown.Append("Here I would add relevant documentation for the consumer of the API including ");
            descriptionMarkdown.Append("[links](https://www.aareon.co.uk) ");
            descriptionMarkdown.Append("and images ![logo](https://www.aareon.co.uk/fm/32/Logo.png)");
            return descriptionMarkdown.ToString();
        }
    }
}
