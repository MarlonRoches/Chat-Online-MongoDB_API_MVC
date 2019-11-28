using Back.Models;
using Back.Servicios;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Back
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
            //Usuarios
            services.Configure<UsuariosDatabaseSettings>(Configuration.GetSection(nameof(UsuariosDatabaseSettings)));
            services.AddSingleton<IUsuariosDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UsuariosDatabaseSettings>>().Value);
            services.AddSingleton<UsuarioServicios>();
            //Mensajes
            services.Configure<MensajesDatabaseSettings>(Configuration.GetSection(nameof(MensajesDatabaseSettings)));
            services.AddSingleton<IMensajesDatabaseSettings>(sp => sp.GetRequiredService<IOptions<MensajesDatabaseSettings>>().Value);
            services.AddSingleton<MensajesServicios>();
          

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
