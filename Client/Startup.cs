using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Client.Data;
using Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client
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
            //services.AddDbContext<ClientContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("ClientDBConnection")));
            services.AddDbContext<ClientContext>(opt => opt.UseInMemoryDatabase("InMemDB"));
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IClientRepo, EfcClientRepo>();
            
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ClientContext context, IClientRepo repository)
        {
            //context.Database.Migrate();
            AddLogEntry(Configuration["ClientName"], repository);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddLogEntry(string client, IClientRepo repo)
        {
            Log log = new Log{ Direction="Outbound", Client=client, Message="I'm Alive!", Timestamp=DateTime.Now};
            repo.CreateLog(log);
            repo.SaveChanges();
        }
    }
}
