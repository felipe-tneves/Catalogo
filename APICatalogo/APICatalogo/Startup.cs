using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Extensions;
using ApiCatalogo.Filters;
using ApiCatalogo.Repository;
using APICatalogo.Context;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //criando uma nova configura��o 
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            //habilitando o mapeamento 
            IMapper mapper = mappingConfig.CreateMapper();
            //registrando como servi�o
            services.AddSingleton(mapper);

            //services.AddScoped<ApiLoggingFilter>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            //habilita e inclui o identity no projeto 
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling
                        = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //The default HSTS value is 30 days. 
                //You may want to change this for production
                //scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //adiciona o middleware de tratamento de erros
            //app.ConfigureExceptionHandler();

            //adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            //adiciona o middleware de roteamento 
            app.UseRouting();

            //adicionando o middleware de autentica��o
            app.UseAuthentication();

            //adicionando o middleware que habilita a autoriza��o 
            app.UseAuthorization();

            //adiciona o middleware que habilita a autorizacao
            app.UseAuthorization();

            //Adiciona o middleware que executa o endpoint 
            //do request atual
            app.UseEndpoints(endpoints =>
            {
                // adiciona os endpoints para as Actions
                // dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}