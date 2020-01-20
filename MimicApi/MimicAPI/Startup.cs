using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimicAPI.Database;
using MimicAPI.Versao1.Repositorios;
using MimicAPI.Versao1.Repositorios.Interfaces;
using AutoMapper;
using MimicAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MimicAPI.Helpers.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

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

                //Pode-se receber a versão pelo cabeçalho da API.
                //cfg.ApiVersionReader = new HeaderApiVersionReader("api-version");

                //Assume a versão padrão quando esta não for especificado na querystring.
                cfg.AssumeDefaultVersionWhenUnspecified = true; 
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });

            //Repository pattern
            services.AddScoped<IPalavraRepositorio, PalavraRepositorio>();

            services.AddSwaggerGen(cfg => {
                cfg.ResolveConflictingActions(apiDescription =>  apiDescription.First());
                
                //A ordem foi invertida com a mais recente no topo
                cfg.SwaggerDoc("v2.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V2.0",
                    Version = "v2.0"
                });
                cfg.SwaggerDoc("v1.1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V1.1",
                    Version = "v1.1"
                });
                cfg.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V1.0",
                    Version = "v1.0"
                });
                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoArquivoXmlComentario = Path.Combine(caminhoProjeto, nomeProjeto);

                cfg.IncludeXmlComments(caminhoArquivoXmlComentario);
                cfg.OperationFilter<ApiVersionOperationFilter>();
            });
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
            app.UseSwagger();// /swagger/v1/swagger.json
            app.UseSwaggerUI(cfg => {
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI - V2.0");
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI - V1.1");
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI - V1.0");
                cfg.RoutePrefix = string.Empty;
                });
        }
    }
}
