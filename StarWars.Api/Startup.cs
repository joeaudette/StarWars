using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;
using StarWars.Api.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Seed;
using Microsoft.EntityFrameworkCore;
using StarWars.Data.EntityFramework.Repositories;
using GraphQL.Types;
using GraphQL;
using StarWars.Core.Logic;
using Microsoft.AspNetCore.Mvc;

namespace StarWars.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {

            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<StarWarsQuery>();            
            services.AddTransient<ICharacterRepository, CharacterRepository>();
            services.AddTransient<IDroidRepository, DroidRepository>();
            services.AddTransient<IHumanRepository, HumanRepository>();
            services.AddTransient<IEpisodeRepository, EpisodeRepository>();
            if (Env.IsEnvironment("Test"))
            {
                services.AddDbContext<StarWarsContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "StarWars"));
            }
            else
            {
                services.AddDbContext<StarWarsContext>(options =>
                    options.UseSqlServer(Configuration["ConnectionStrings:StarWarsDatabaseConnection"]));
            }
            services.AddScoped<IDocumentExecuter, DocumentExecuter>();
            services.AddScoped<ITrilogyHeroes, TrilogyHeroes>();
            services.AddTransient<DroidType>();
            services.AddTransient<HumanType>();
            services.AddTransient<CharacterInterface>();
            services.AddTransient<EpisodeEnum>();
            var sp = services.BuildServiceProvider();
            services.AddScoped<ISchema>(_ => new StarWarsSchema(type => (GraphType) sp.GetService(type)) {Query = sp.GetService<StarWarsQuery>()});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseMvc();

            
        }
    }
}
