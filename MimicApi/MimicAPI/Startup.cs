using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimicAPI.Database;
using MimicAPI.Versao1.Repositorios;
using MimicAPI.Versao1.Repositorios.Interfaces;
using System.IO;
using System.Reflection;
using AutoMapper;
using MimicAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AutoMapper - Configuração
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DtoAutoMapperProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddDbContext<MimicContext>(opt =>
            {
                opt.UseSqlite("Data Source=Database\\Mimic.db");
            });
            services.AddMvc();
            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });

            //Repository pattern
            services.AddScoped<IPalavraRepositorio, PalavraRepositorio>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}
